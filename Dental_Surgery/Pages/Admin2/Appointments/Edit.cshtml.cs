using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.Model;
using Dental.Service;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public EditModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public void OnGet(int id)
        {
            Appointment = _unitOfWork.AppointmentRepo.Get(id);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPost(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.AppointmentRepo.Update(appointment);
                _unitOfWork.Save();
            }
            return RedirectToPage("Index");
        }

        //private bool AppointmentExists(int id)
        //{
        //    return _context.Appointments.Any(e => e.AppointmentId == id);
        //}
    }
}
