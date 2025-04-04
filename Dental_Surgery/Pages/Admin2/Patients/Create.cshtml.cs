using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dental.DataAccess;
using Dental.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Dental_Surgery.Pages.Admin2.Patients
{
	[Authorize(Roles = "Receptionist")]
	public class CreateModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public CreateModel(Dental.DataAccess.AppDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Patient Patient { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Patient.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError("Patient.DateOfBirth", "Date of birth can't be in the future");
                return Page();
            }
            // Check for duplicate patient by PPS, email, or full name + DOB (excluding the current record)
            bool patientExists = await _context.Patients
                .AnyAsync(p => (p.PPS == Patient.PPS ||
                               p.Email == Patient.Email ||
                               (p.FirstName == Patient.FirstName &&
                                p.LastName == Patient.LastName &&
                                p.DateOfBirth == Patient.DateOfBirth))
                                && p.PatientId != Patient.PatientId); // Exclude the current patient

            if (patientExists)
            {
                ModelState.AddModelError(string.Empty, "A patient with this PPS, email, or name and date of birth already exists.");
                return Page();
            }

            _context.Patients.Add(Patient);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
