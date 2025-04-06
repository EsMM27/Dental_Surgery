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

        public string NextAvailableTime { get; set; } = "—";


        // Schedule
        public List<Appointment> DailyAppointments { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public DateTime ScheduleDate { get; set; } = DateTime.Today;
        [BindProperty(SupportsGet = true)]
        public string? PatientSearchTerm { get; set; }
        public Dentist Dentist { get; private set; }
        public bool IsDentist { get; private set; }

        public List<Appointment> RecentAppointmentsForNextPatient { get; set; } = new();



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
        public List<string> NextAvailableSlots { get; set; } = new();

        private TimeSpan RoundUpToNextSlot(TimeSpan time, int intervalMinutes)
        {
            int totalMinutes = (int)Math.Ceiling(time.TotalMinutes / intervalMinutes) * intervalMinutes;
            return TimeSpan.FromMinutes(totalMinutes);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            int daysToSearch = 14;
            TimeSpan openTime = new TimeSpan(9, 0, 0);
            TimeSpan lastSlotTime = new TimeSpan(16, 0, 0);
            int slotMinutes = 30;

            var foundSlots = new List<string>();

            var allDentists = await _unitOfWork.Dentists.GetAllAsync();

            for (int i = 0; i < daysToSearch && foundSlots.Count < 3; i++)
            {
                var date = DateTime.Today.AddDays(i);

                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var appointmentsOnDay = await _unitOfWork.Appointments.GetAppointmentsForDateAsync(date);

                TimeSpan slot = date == DateTime.Today
                    ? (DateTime.Now.TimeOfDay < openTime ? openTime : RoundUpToNextSlot(DateTime.Now.TimeOfDay, slotMinutes))
                    : openTime;

                while (slot <= lastSlotTime && foundSlots.Count < 3)
                {
                    if (slot >= new TimeSpan(11, 30, 0) && slot < new TimeSpan(13, 0, 0))
                    {
                        slot = new TimeSpan(13, 0, 0);
                        continue;
                    }

                    // Find first available dentist for this slot
                    var dentistsBooked = appointmentsOnDay
                        .Where(a => a.AppointmentDate.TimeOfDay == slot)
                        .Select(a => a.DentistId)
                        .ToHashSet();

                    var availableDentist = allDentists.FirstOrDefault(d => !dentistsBooked.Contains(d.DentistId));

                    if (availableDentist != null)
                    {
                        string formatted = $"{date:dd/MM} {date:dddd} at {slot:hh\\:mm} (Dr. {availableDentist.LastName})";
                        foundSlots.Add(formatted);
                    }

                    slot = slot.Add(TimeSpan.FromMinutes(slotMinutes));
                }
            }

            NextAvailableSlots = foundSlots;

            if (!NextAvailableSlots.Any())
            {
                NextAvailableSlots.Add("No availability in next 2 weeks");
            }







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

                        // Load last 3 historical appointments for that patient with this dentist
                        if (nextAppointment.PatientId != null)
                        {
                            RecentAppointmentsForNextPatient = (await _unitOfWork.Appointments.GetByConditionAsync(a =>
                                a.PatientId == nextAppointment.PatientId &&
                                a.DentistId == Dentist.DentistId &&
                                a.AppointmentDate < nextAppointment.AppointmentDate))
                                .OrderByDescending(a => a.AppointmentDate)
                                .Take(3)
                                .ToList();
                        }
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
