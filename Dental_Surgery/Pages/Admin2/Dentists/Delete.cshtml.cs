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

		public IActionResult OnPost()
		{

			var dentistInDb = _unitOfWork.DentistRepo.Get(Dentist.DentistId);
			if (dentistInDb == null)
			{
				return NotFound();
			}

			_unitOfWork.DentistRepo.Delete(dentistInDb);
			_unitOfWork.Save();

			return RedirectToPage("Index");
		}

	}
}
