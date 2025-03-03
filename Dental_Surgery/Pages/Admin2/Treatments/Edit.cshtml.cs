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

namespace Dental_Surgery.Pages.Admin2.Treatments
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Treatment Treatment { get; set; } = default!;

        public void OnGet(int id)
        {
            Treatment = _unitOfWork.TreatmentRepo.Get(id);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPost(Treatment treatment)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.TreatmentRepo.Update(treatment);
                _unitOfWork.Save();
            }
            return RedirectToPage("Index");
        }

        //private bool TreatmentExists(int id)
        //{
        //    return _context.Treatments.Any(e => e.TreatmentId == id);
        //}
    }
}
