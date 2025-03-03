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

namespace Dental_Surgery.Pages.Admin2.Dentists
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Dentist Dentist { get; set; } = default!;

        public void OnGet(int id)
        {
            Dentist = _unitOfWork.DentistRepo.Get(id);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPost(Dentist dentist)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.DentistRepo.Update(dentist);
                _unitOfWork.Save();
            }
            return RedirectToPage("Index");
        }

        //    private bool DentistExists(int id)
        //{
        //    return _unitOfWork.DentistRepo.Any(e => e.DentistId == id);
        //}
    }
}
