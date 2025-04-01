using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Dental.Model;
using Dental.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Surgery.Pages.Shared
{
    [Authorize]
    public class ScheduleModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public ScheduleModel(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public List<Appointment> DailyAppointments { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public DateTime ScheduleDate { get; set; } = DateTime.Today;
        [BindProperty(SupportsGet = true)]
        public string? PatientSearchTerm { get; set; }
        public Dentist Dentist { get; private set; }
		public bool IsDentist { get; private set; }

		private DateTime GetPreviousWeekday(DateTime date)
		{
			do
			{
				date = date.AddDays(-1);
			} while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
			return date;
		}

		private DateTime GetNextWeekday(DateTime date)
		{
			do
			{
				date = date.AddDays(1);
			} while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
			return date;
		}

		public DateTime PreviousWeekday => GetPreviousWeekday(ScheduleDate);
		public DateTime NextWeekday => GetNextWeekday(ScheduleDate);
		public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            IsDentist = roles.Contains("Dentist");

            if (IsDentist)
            {
                Dentist = (await _unitOfWork.Dentists.GetAllAsync())
                              .FirstOrDefault(d => d.UserId == user.Id);
                if (Dentist != null)
                {
                    DailyAppointments = (await _unitOfWork.Appointments
                        .GetAppointmentsForDentistAsync(Dentist.DentistId, ScheduleDate))
                        .OrderBy(a => a.AppointmentDate.TimeOfDay)
                        .ToList();
                }
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
        }
    }
}