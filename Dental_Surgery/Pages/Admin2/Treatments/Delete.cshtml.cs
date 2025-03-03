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

namespace Dental_Surgery.Pages.Admin2.Treatments
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Treatment Treatment { get; set; } = default!;

        public void OnGet(int id)
        {
            Treatment = _unitOfWork.TreatmentRepo.Get(id);
        }

        public IActionResult OnPost(Treatment treatment)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.TreatmentRepo.Delete(treatment);
                _unitOfWork.Save();
            }
            return RedirectToPage("Index");
        }
    }
}
