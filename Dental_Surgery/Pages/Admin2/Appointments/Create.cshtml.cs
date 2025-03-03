using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dental.DataAccess;
using Dental.Model;
using Dental.Service;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
    public class CreateModel : PageModel
    {
        
        private readonly IUnitOfWork _unitOfWork;
        public CreateModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult OnGet()
        {
            //ViewData["DentistId"] = new SelectList(_unitOfWork.Dentist, "DentistId", "FirstName");
            //ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName");
            //ViewData["TreatmentId"] = new SelectList(_context.Treatments, "TreatmentId", "Name");
            return Page();
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _unitOfWork.AppointmentRepo.Add(Appointment);
            _unitOfWork.Save();

            return RedirectToPage("./Index");
        }
    }
}
