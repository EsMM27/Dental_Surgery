using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dental.Model;
using Dental.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dental_Surgery.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public string DisplayName { get; set; }

        // Dashboard
        public int AppointmentsToday { get; set; }
        public int DentistsOnDuty { get; set; } // For Receptionist
        public string NextAppointmentTime { get; set; }
        public string NextPatientName { get; set; }

        // Schedule
        public List<Appointment> DailyAppointments { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public DateTime ScheduleDate { get; set; } = DateTime.Today;
        [BindProperty(SupportsGet = true)]
        public string? PatientSearchTerm { get; set; }
        public Dentist Dentist { get; private set; }
        public bool IsDentist { get; private set; }


        private DateTime GetPreviousWeekday(DateTime date)
        {
            do { date = date.AddDays(-1); }
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
            return date;
        }

        private DateTime GetNextWeekday(DateTime date)
        {
            do { date = date.AddDays(1); }
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
            return date;
        }

        public DateTime PreviousWeekday => GetPreviousWeekday(ScheduleDate);
        public DateTime NextWeekday => GetNextWeekday(ScheduleDate);

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            IsDentist = roles.Contains("Dentist");

            if (roles.Contains("Admin"))
            {
                return RedirectToPage("/Admin2/Analytics/Index");
            }

            if (roles.Contains("Receptionist"))
            {
                DisplayName = user?.UserName ?? "Receptionist";
                var today = DateTime.Today;
                var appointments = await _unitOfWork.Appointments.GetAppointmentsForDateAsync(today);
                AppointmentsToday = appointments.Count();
                DentistsOnDuty = appointments.Select(a => a.DentistId).Distinct().Count();
            }
            else if (IsDentist)
            {
                Dentist = (await _unitOfWork.Dentists.GetAllAsync()).FirstOrDefault(d => d.UserId == user.Id);

                if (Dentist != null)
                {
                    DisplayName = $"Dr. {Dentist.FirstName} {Dentist.LastName}";

                    var today = DateTime.Today;
                    var todaysAppointments = (await _unitOfWork.Appointments
                        .GetAppointmentsForDentistAsync(Dentist.DentistId, today))
                        .OrderBy(a => a.AppointmentDate.TimeOfDay)
                        .ToList();

                    AppointmentsToday = todaysAppointments.Count;

                    var nextAppointment = todaysAppointments
                        .FirstOrDefault(a => a.AppointmentDate.TimeOfDay > DateTime.Now.TimeOfDay);

                    if (nextAppointment != null)
                    {
                        NextAppointmentTime = nextAppointment.AppointmentDate.ToString("HH:mm");
                        NextPatientName = $"{nextAppointment.Patient?.FirstName} {nextAppointment.Patient?.LastName}";
                    }
                    else
                    {
                        NextAppointmentTime = "—";
                        NextPatientName = "No more appointments today";
                    }
                }
                else
                {
                    DisplayName = "Dentist";
                }
            }

            // Load schedule for today
            if (IsDentist && Dentist != null)
            {
                DailyAppointments = (await _unitOfWork.Appointments
                    .GetAppointmentsForDentistAsync(Dentist.DentistId, ScheduleDate))
                    .OrderBy(a => a.AppointmentDate.TimeOfDay)
                    .ToList();
            }
            else if (roles.Contains("Receptionist"))
            {
                var appointments = await _unitOfWork.Appointments
                    .GetAppointmentsForDateAsync(ScheduleDate);

                DailyAppointments = appointments
                    .OrderBy(a => a.AppointmentDate.TimeOfDay)
                    .ToList();

                if (!string.IsNullOrEmpty(PatientSearchTerm))
                {
                    DailyAppointments = DailyAppointments
                        .Where(a => a.Patient != null &&
                              (a.Patient.FirstName + " " + a.Patient.LastName)
                              .Contains(PatientSearchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            return Page();
        }
    }
}
