using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.Model;
using Dental.Service;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public void OnGet(int id)
        {
            Appointment = _unitOfWork.AppointmentRepo.Get(id);
        }

        public IActionResult OnPost(Appointment appointment)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.AppointmentRepo.Delete(appointment);
                _unitOfWork.Save();
            }
            return RedirectToPage("Index");
        }
    }
}
