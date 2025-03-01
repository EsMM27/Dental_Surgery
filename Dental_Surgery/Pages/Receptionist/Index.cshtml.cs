using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dental.Model;
using Dental.Service;

namespace Dental_Surgery.Pages.Receptionist
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dental.Model;
    using Dental.Service;

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
            public Appointment Appointment { get; set; }

            [BindProperty]
            public string SelectedTime { get; set; }

            public List<Dentist> Dentists { get; set; } = new List<Dentist>();
            public List<(string TimeSlot, bool IsBooked)> TimeSlotsWithAvailability { get; set; } = new List<(string, bool)>();

            public async Task<IActionResult> OnGetAsync()
            {
                Dentists = (List<Dentist>)await _unitOfWork.Dentists.GetAllAsync();

                if (Appointment == null)
                {
                    Appointment = new Appointment { AppointmentDate = DateTime.Today };
                }

                if (Appointment.DentistId > 0)
                {
                    TimeSlotsWithAvailability = await _unitOfWork.Appointments.GetTimeSlotsWithAvailabilityAsync(
                        Appointment.DentistId,
                        Appointment.AppointmentDate
                    );
                }

                return Page();
            }

            public async Task<IActionResult> OnPostFetchAvailabilityAsync()
            {
                Dentists = (List<Dentist>)await _unitOfWork.Dentists.GetAllAsync();

                if (Appointment.DentistId <= 0)
                {
                    TempData["ErrorMessage"] = "Please select a dentist.";
                    return Page();
                }

                TimeSlotsWithAvailability = await _unitOfWork.Appointments.GetTimeSlotsWithAvailabilityAsync(
                    Appointment.DentistId,
                    Appointment.AppointmentDate
                );

                return Page();
            }

            public async Task<IActionResult> OnPostCreateAppointmentAsync()
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                if (string.IsNullOrEmpty(SelectedTime))
                {
                    ModelState.AddModelError("SelectedTime", "Please select a time slot.");
                    return Page();
                }

                try
                {
                    var time = TimeSpan.Parse(SelectedTime);
                    Appointment.AppointmentDate = Appointment.AppointmentDate.Date + time;
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex, "Invalid time format");
                    ModelState.AddModelError("SelectedTime", "Invalid time format.");
                    return Page();
                }

                try
                {
                    await _unitOfWork.Appointments.AddAsync(Appointment);
                    await _unitOfWork.SaveAsync();

                    TempData["SuccessMessage"] = "Appointment created successfully!";
                    return RedirectToPage();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating appointment");
                    TempData["ErrorMessage"] = "Error creating appointment: " + ex.Message;
                    return Page();
                }
            }
        }
    }
}