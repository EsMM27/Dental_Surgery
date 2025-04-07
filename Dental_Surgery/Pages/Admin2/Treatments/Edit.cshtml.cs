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

namespace Dental_Surgery.Pages.Admin2.Treatments
{
	[Authorize(Roles = "Admin")]
	public class EditModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public EditModel(Dental.DataAccess.AppDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Treatment Treatment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment =  await _context.Treatments.FirstOrDefaultAsync(m => m.TreatmentId == id);
            if (treatment == null)
            {
                return NotFound();
            }
            Treatment = treatment;
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

            _context.Attach(Treatment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TreatmentExists(Treatment.TreatmentId))
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

        private bool TreatmentExists(int id)
        {
            return _context.Treatments.Any(e => e.TreatmentId == id);
        }
    }
}
