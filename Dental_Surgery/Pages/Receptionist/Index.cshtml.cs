
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dental.Model;
    using Dental.Service;
    using Dental_Surgery.Utilities;


namespace Dental_Surgery.Pages.Receptionist
    {
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IUnitOfWork unitOfWork, ILogger<IndexModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment { AppointmentDate = DateTime.Today };

        [BindProperty]
        public string SelectedTime { get; set; }

        public List<Dentist> Dentists { get; set; } = new();
        public List<(string TimeSlot, bool IsBooked)> TimeSlotsWithAvailability { get; set; } = new();
        public List<Patient> Patients { get; set; } = new();
        public List<Treatment> Treatments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDataAsync();
            await LoadWeeklyAvailabilityAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostFetchAvailabilityAsync()
        {
            await LoadDataAsync();
            await LoadWeeklyAvailabilityAsync();
            return Page();  
        }

        private async Task LoadWeeklyAvailabilityAsync()
        {
            if (Appointment.DentistId > 0)
            {
                var startOfWeek = Appointment.AppointmentDate.StartOfWeek(DayOfWeek.Monday);
                var endOfWeek = startOfWeek.AddDays(4); // Monday to Friday

                TimeSlotsWithAvailability = new List<(string TimeSlot, bool IsBooked)>();

                for (var date = startOfWeek; date <= endOfWeek; date = date.AddDays(1))
                {
                    var dailySlots = await _unitOfWork.Appointments.GetTimeSlotsWithAvailabilityAsync(Appointment.DentistId, date);
                    var availableTimeSlots = TimeSlots.GetAvailableTimeSlots(date);

                    TimeSlotsWithAvailability.AddRange(dailySlots
                        .Where(ts => availableTimeSlots.Contains(ts.TimeSlot))
                        .Select(ts => (TimeSlot: $"{date:yyyy-MM-dd} {ts.TimeSlot}", ts.IsBooked)));
                }
            }
        }

        

        public async Task<IActionResult> OnPostCreateAppointmentAsync()
        {
            if (!ValidateAppointment())
            {
                return Page();
            }

            var timeParts = SelectedTime.Split(':');
            Appointment.AppointmentDate = new DateTime(
                Appointment.AppointmentDate.Year,
                Appointment.AppointmentDate.Month,
                Appointment.AppointmentDate.Day,
                int.Parse(timeParts[0]),
                int.Parse(timeParts[1]),
                0
            );

            await _unitOfWork.Appointments.AddAsync(Appointment);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Appointment created successfully.";
            return Page();
        }

        private async Task LoadDataAsync()
        {
            Dentists = (List<Dentist>)await _unitOfWork.Dentists.GetAllAsync();
            Patients = (List<Patient>)await _unitOfWork.Patients.GetAllAsync();
            Treatments = (List<Treatment>)await _unitOfWork.Treatments.GetAllAsync();
        }

        private bool ValidateAppointment()
        {
            if (Appointment.DentistId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a dentist.";
                return false;
            }

            if (Appointment.PatientId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a patient.";
                return false;
            }

            if (Appointment.TreatmentId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a treatment.";
                return false;
            }

            if (string.IsNullOrEmpty(SelectedTime))
            {
                TempData["ErrorMessage"] = "Please select a time.";
                return false;
            }

            return true;
        }
    }
    }
public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}