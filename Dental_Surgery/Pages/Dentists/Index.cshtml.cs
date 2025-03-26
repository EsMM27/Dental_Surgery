using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dental.Model;
using Dental.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace Dental_Surgery.Pages.Dentists
{
    [Authorize(Roles = "Dentist")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ILogger<IndexModel> logger, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public Dentist CurrentDentist { get; set; }
        public List<Appointment> DailyAppointments { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime ScheduleDate { get; set; } = DateTime.Today;

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User); //getting the id of the dentist currently logged in
            var currentDentist = (await _unitOfWork.Dentists.GetAllAsync())
                        .FirstOrDefault(d => d.UserId == userId);

            CurrentDentist = currentDentist;

            if (currentDentist != null)
            {
                DailyAppointments = (await _unitOfWork.Appointments
                    .GetAppointmentsForDentistAsync(currentDentist.DentistId, ScheduleDate))
                    .OrderBy(a => a.AppointmentDate.TimeOfDay)
                    .ToList();
            }
        }
    }
}
