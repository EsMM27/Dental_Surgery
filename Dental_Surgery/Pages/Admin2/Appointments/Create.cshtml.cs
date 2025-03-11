using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dental.DataAccess;
using Dental.Model;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
    public class CreateModel : PageModel
    {
        private readonly AppDBContext _context;

        public CreateModel(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["DentistId"] = new SelectList(_context.Dentists, "DentistId", "FirstName");
            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "TreatmentId", "Name");
            return Page();
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

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

        // AJAX endpoint to search patients
        public async Task<JsonResult> OnGetSearchPatients(string searchString)
        {
            var patients = await _context.Patients
                .Where(p => string.IsNullOrEmpty(searchString) ||
                            p.FirstName.Contains(searchString) ||
                            p.LastName.Contains(searchString) ||
                            p.PPS.Contains(searchString))
                .Select(p => new { p.PPS, p.FirstName, p.LastName })
                .ToListAsync();

            return new JsonResult(patients);
        }
    }
}
