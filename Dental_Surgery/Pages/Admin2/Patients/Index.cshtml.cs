using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.Model;

namespace Dental_Surgery.Pages.Admin2.Patients
{
    public class IndexModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public IndexModel(Dental.DataAccess.AppDBContext context)
        {
            _context = context;
        }

        public IList<Patient> Patient { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public async Task OnGetAsync()
        {
            IQueryable<Patient> patientQuery = _context.Patients;

            if (!string.IsNullOrEmpty(SearchString))
            {
                patientQuery = patientQuery.Where(p =>
                    p.FirstName.Contains(SearchString) ||
                    p.LastName.Contains(SearchString) ||
                    p.PPS.Contains(SearchString));
            }

            Patient = await patientQuery.ToListAsync();
        }
        public async Task<JsonResult> OnGetSearchPatients(string searchString)
        {
            var patients = await _context.Patients
                .Where(p => string.IsNullOrEmpty(searchString) ||
                            p.FirstName.Contains(searchString) ||
                            p.LastName.Contains(searchString) ||
                            p.PPS.Contains(searchString))
                .Select(p => new {
                    p.FirstName,
                    p.LastName,
                    p.PPS,
                    p.ContactNumber,
                    p.Email,
                    p.Address,
                    DateOfBirth = p.DateOfBirth.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            return new JsonResult(patients);
        }
    }
}
