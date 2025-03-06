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
using System.Numerics;

namespace Dental_Surgery.Pages.Admin2.Dentists
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Dentist Dentist { get; set; } = default!;

        public void OnGet(int id)
        {
            Dentist = _unitOfWork.DentistRepo.Get(id);
        }

        public IActionResult OnPost(Dentist Dentist)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.DentistRepo.Delete(Dentist);
                _unitOfWork.Save();
            }
            return RedirectToPage("Index");
        }
    }
}
