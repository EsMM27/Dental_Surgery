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
using Microsoft.AspNetCore.Authorization;

namespace Dental_Surgery.Pages.Admin2.Patients
{
	[Authorize(Roles = "Admin,Receptionist,Dentist")]
	public class EditModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public EditModel(Dental.DataAccess.AppDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Patient Patient { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient =  await _context.Patients.FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }
            Patient = patient;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
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


            _context.Attach(Patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(Patient.PatientId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}
