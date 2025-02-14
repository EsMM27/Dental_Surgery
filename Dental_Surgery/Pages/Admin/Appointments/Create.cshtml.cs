using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dental.DataAccess;
using Dental.Model;

namespace Dental_Surgery.Pages.Admin.Appointments
{
    public class CreateModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public CreateModel(Dental.DataAccess.AppDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["DentistId"] = new SelectList(
                _context.Dentists.Select(d => new { d.DentistId, FullName = d.FirstName + " " + d.LastName }),
                "DentistId", "FullName"
            );

            ViewData["PatientId"] = new SelectList(
                _context.Patients.Select(p => new { p.PatientId, FullName = p.FirstName + " " + p.LastName }),
                "PatientId", "FullName"
            );

            ViewData["TreatmentId"] = new SelectList(
                _context.Treatments, "TreatmentId", "Name"
            );

            return Page();
        }
        //a way to dispaly the data on the page

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Appointments.Add(Appointment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
