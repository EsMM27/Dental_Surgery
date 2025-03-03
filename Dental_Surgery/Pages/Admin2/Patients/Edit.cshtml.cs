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

namespace Dental_Surgery.Pages.Admin2.Patients
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Patient Patient { get; set; } = default!;

        public void OnGet(int id)
        {
            Patient = _unitOfWork.PatientRepo.Get(id);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPost(Patient patient)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.PatientRepo.Update(patient);
                _unitOfWork.Save();
            }
            return RedirectToPage("Index");
        }

        //private bool PatientExists(int id)
        //{
        //    return _context.Patients.Any(e => e.PatientId == id);
        //}
    }
}
