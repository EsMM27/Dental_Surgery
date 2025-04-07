using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.Model;
using Microsoft.AspNetCore.Authorization;
using Dental.Service;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
	[Authorize(Roles = "Admin,Receptionist,Dentist")]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Appointment> Appointment { get; set; }

        public async Task OnGetAsync()
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync();
            Appointment = appointments.OrderByDescending(a => a.AppointmentDate).ToList();
        }
    }
}
