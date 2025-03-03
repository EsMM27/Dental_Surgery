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
        public IEnumerable<Patient> Patients;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public void OnGet()
        {
            //var patientQuery = _unitOfWork.PatientRepo.GetAll().AsQueryable(); // Ensure it's IQueryable

            //if (!string.IsNullOrEmpty(SearchString))
            //{
            //    patientQuery = patientQuery.Where(p =>
            //        EF.Functions.Like(p.FirstName, $"%{SearchString}%") ||
            //        EF.Functions.Like(p.LastName, $"%{SearchString}%") ||
            //        EF.Functions.Like(p.PPS, $"%{SearchString}%"));  // Using LIKE for case-insensitive search
            //}

            //Patients = patientQuery.ToList(); // Execute query
            Patients = _unitOfWork.PatientRepo.GetAll();
        }




        //public void OnGet()
        //{
        //    Patients = _unitOfWork.PatientRepo.GetAll();
        //}
    }
}
