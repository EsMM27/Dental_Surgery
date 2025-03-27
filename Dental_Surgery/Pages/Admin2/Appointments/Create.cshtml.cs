using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dental.DataAccess;
using Dental.Model;
using Dental.Service;
using Dental_Surgery.Utilities;


namespace Dental_Surgery.Pages.Admin2.Appointments
{
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IndexModel> _logger;

        public CreateModel(IUnitOfWork unitOfWork, ILogger<IndexModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            TimeSlotsWithAvailability = new List<(DateTime Date, string TimeSlot, bool IsBooked)>();
            SelectedTime = string.Empty;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment { AppointmentDate = DateTime.Today };

        [BindProperty]
        public string SelectedTime { get; set; }
        [BindProperty]
        public string PatientSearchQuery { get; set; }

        public List<Dentist> Dentists { get; set; } = new();
        public List<(DateTime Date, string TimeSlot, bool IsBooked)> TimeSlotsWithAvailability { get; set; }
        public List<Patient> Patients { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            //return RedirectToPage("/Shared/Schedule"); 

            _logger.LogInformation("OnGetAsync called");
            await LoadDataAsync();

            if (Appointment.DentistId > 0)
            {
                await LoadTimeSlotsAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostFetchAvailabilityAsync()
        {
            _logger.LogInformation("OnPostFetchAvailabilityAsync called");
            await LoadDataAsync();

            if (Appointment.DentistId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a dentist.";
                _logger.LogWarning("No dentist selected");
                return Page();
            }

            // Fetch time slots for the entire week based on the selected Monday
            var startOfWeek = Appointment.AppointmentDate.StartOfWeek(DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(4);

            TimeSlotsWithAvailability = new List<(DateTime Date, string TimeSlot, bool IsBooked)>();

            for (var date = startOfWeek; date <= endOfWeek; date = date.AddDays(1))
            {
                var dailySlots = await _unitOfWork.Appointments.GetTimeSlotsWithAvailabilityAsync(Appointment.DentistId, date);
                TimeSlotsWithAvailability.AddRange(dailySlots.Select(slot => (date, slot.TimeSlot, slot.IsBooked)));
            }

            if (Appointment.PatientId > 0)
            {
                var patient = await _unitOfWork.Patients.GetByIdAsync(Appointment.PatientId);
                if (patient != null)
                {
                    PatientSearchQuery = $"{patient.FirstName} {patient.LastName} ({patient.PPS})";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAppointmentAsync()
        {
            _logger.LogInformation("OnPostCreateAppointmentAsync called");
            if (!ValidateAppointment())
            {
                await LoadDataAsync();
                return Page();
            }

            // Parse the selected time and set the appointment date and time
            var timeParts = SelectedTime.Split(':');
            Appointment.AppointmentDate = new DateTime(
                Appointment.AppointmentDate.Year,
                Appointment.AppointmentDate.Month,
                Appointment.AppointmentDate.Day,
                int.Parse(timeParts[0]),
                int.Parse(timeParts[1]),
                0
            );

            _logger.LogInformation("Creating appointment for PatientId: {PatientId}, DentistId: {DentistId}, Date: {Date}, Time: {Time}",
                Appointment.PatientId, Appointment.DentistId, Appointment.AppointmentDate, SelectedTime);

            await _unitOfWork.Appointments.AddAsync(Appointment);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Appointment created successfully.";
            return RedirectToPage("/Admin2/Appointments/Index");
        }

        private async Task LoadDataAsync()
        {
            _logger.LogInformation("Loading data");
            Dentists = (await _unitOfWork.Dentists.GetAllAsync()).ToList();
            Patients = (await _unitOfWork.Patients.GetAllAsync()).ToList();
        }

        private async Task LoadTimeSlotsAsync()
        {
            _logger.LogInformation("Loading time slots for DentistId: {DentistId}, Date: {Date}", Appointment.DentistId, Appointment.AppointmentDate);
            var timeSlots = await _unitOfWork.Appointments.GetTimeSlotsWithAvailabilityAsync(
                Appointment.DentistId,
                Appointment.AppointmentDate
            );

            // Filter available time slots
            var availableTimeSlots = TimeSlots.GetAvailableTimeSlots(Appointment.AppointmentDate);
            TimeSlotsWithAvailability = timeSlots
                .Where(ts => availableTimeSlots.Contains(ts.TimeSlot))
                .Select(ts => (Appointment.AppointmentDate, ts.TimeSlot, ts.IsBooked))
                .ToList();
        }

        private bool ValidateAppointment()
        {
            if (Appointment.DentistId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a dentist.";
                _logger.LogWarning("Validation failed: No dentist selected");
                return false;
            }

            if (Appointment.PatientId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a patient.";
                _logger.LogWarning("Validation failed: No patient selected");
                return false;
            }

            if (string.IsNullOrEmpty(SelectedTime))
            {
                TempData["ErrorMessage"] = "Please select a time.";
                _logger.LogWarning("Validation failed: No time selected");
                return false;
            }

            return true;
        }

        public async Task<IActionResult> OnGetSearchPatientsAsync(string query)
        {
            _logger.LogInformation("OnGetSearchPatientsAsync called with query: {Query}", query);
            if (string.IsNullOrEmpty(query))
            {
                return new JsonResult(new List<Patient>());
            }

            // Fetch all patients and filter based on query (by name or PPS)
            var allPatients = (await _unitOfWork.Patients.GetAllAsync()).ToList();

            var filteredPatients = allPatients
                .Where(p => (p.FirstName != null && p.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                            (p.LastName != null && p.LastName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                            (p.PPS != null && p.PPS.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .Select(p => new { p.PatientId, p.FirstName, p.LastName, p.PPS })
                .ToList();

            return new JsonResult(filteredPatients);
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
}