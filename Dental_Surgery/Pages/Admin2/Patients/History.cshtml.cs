using Dental.Model;
using Dental.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dental_Surgery.Pages.Admin2.Patients
{
    [Authorize(Roles = "Dentist")]
    public class HistoryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public HistoryModel(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public List<Appointment> AppointmentHistory { get; set; } = new();
        public string DentistName { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var dentist = (await _unitOfWork.Dentists.GetAllAsync())
                          .FirstOrDefault(d => d.UserId == user.Id);

            if (dentist != null)
            {
                DentistName = $"{dentist.FirstName} {dentist.LastName}";


                var history = await _unitOfWork.Appointments
                    .GetAppointmentHistoryForDentistAsync(dentist.DentistId);

                AppointmentHistory = history.ToList();
            }
        }
    }
}