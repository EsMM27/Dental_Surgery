﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.Model;
using Dental.Service;

namespace Dental_Surgery.Pages.Admin2.Patients
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Patient Patient { get; set; } = default!;

        public void OnGet(int id)
        {
            Patient = _unitOfWork.PatientRepo.Get(id);
        }

		public IActionResult OnPost()
		{

			var patientInDb = _unitOfWork.PatientRepo.Get(Patient.PatientId);
			if (patientInDb == null)
			{
				return NotFound();
			}

			_unitOfWork.PatientRepo.Delete(patientInDb);
			_unitOfWork.Save();

			return RedirectToPage("Index");
		}
	}
}
