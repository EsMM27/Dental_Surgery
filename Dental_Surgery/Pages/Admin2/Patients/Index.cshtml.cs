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

namespace Dental_Surgery.Pages.Admin2.Patients
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public IEnumerable<Patient> Patients { get; set; } = new List<Patient>();

		public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

		public void OnGet(string? searchString)
		{
			if (!string.IsNullOrEmpty(searchString))
			{
				Patients = _unitOfWork.PatientRepo
					.GetAll()
					.Where(p => p.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
								p.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
								p.PPS.Contains(searchString, StringComparison.OrdinalIgnoreCase))
					.ToList();
			}
			else
			{
				Patients = _unitOfWork.PatientRepo.GetAll();
			}
		}

		public JsonResult OnGetPatientsJson(string searchString)
		{
			var patients = _unitOfWork.PatientRepo
				.GetAll()
				.Where(p => string.IsNullOrEmpty(searchString) ||
							p.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
							p.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
							p.PPS.Contains(searchString, StringComparison.OrdinalIgnoreCase))
				.Select(p => new {
					p.PatientId,
					p.PPS,
					p.FirstName,
					p.LastName,
					p.ContactNumber,
					p.Email,
					p.Address,
					p.DateOfBirth
				})
				.ToList();

			return new JsonResult(patients);
		}
	}

	//public void OnGet()
	//{
	//    Patients = _unitOfWork.PatientRepo.GetAll();
	//}
}

