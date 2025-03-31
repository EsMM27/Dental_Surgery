using System.Linq;
using Dental.DataAccess;
using Dental.Model;
using Dental.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

        // Shared
        public int AppointmentsToday { get; set; }

        // Receptionist
        public int DentistsOnDuty { get; set; }
        public int NewPatientsToday { get; set; }

        // Dentist
        public string NextAppointmentTime { get; set; }
        public string NextPatientName { get; set; }

        // Admin
        public int TotalUsers { get; set; }
        public int TotalDentists { get; set; }
        public int TotalReceptionists { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            DisplayName = user?.UserName ?? "Receptionist";

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Receptionist"))
            {
                var today = DateTime.Today;

                // 1. Appointments Today
                var appointments = await _unitOfWork.Appointments.GetAppointmentsForDateAsync(today);
                AppointmentsToday = appointments.Count();

                // 2. Dentists On Duty (you could define this as "any dentist who has at least 1 appointment today")
                var dentistIds = appointments.Select(a => a.DentistId).Distinct();
                DentistsOnDuty = dentistIds.Count();

                
            }

            else if (roles.Contains("Dentist"))
            {
                var dentist = (await _unitOfWork.Dentists.GetAllAsync())
                    .FirstOrDefault(d => d.UserId == user.Id);

                if (dentist != null)
                {
                    var today = DateTime.Today;

                    var todaysAppointments = (await _unitOfWork.Appointments
                        .GetAppointmentsForDentistAsync(dentist.DentistId, today))
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
            }

            else if (User.IsInRole("Admin"))
            {
                AppointmentsToday = 20;
                TotalUsers = 10;
                TotalDentists = 5;
                TotalReceptionists = 3;
            }
        }
    }

}
