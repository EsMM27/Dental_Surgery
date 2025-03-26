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

namespace Dental_Surgery.Pages.Admin2.Dentists
{
    public class EditModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public EditModel(Dental.DataAccess.AppDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Dentist Dentist { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dentist =  await _context.Dentists.FirstOrDefaultAsync(m => m.DentistId == id);
            if (dentist == null)
            {
                return NotFound();
            }
            Dentist = dentist;
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

            // Check if a dentist with the same email OR first + last name already exists
            bool dentistExists = await _context.Dentists
                .AnyAsync(d => d.Email == Dentist.Email ||
                               (d.FirstName == Dentist.FirstName && d.LastName == Dentist.LastName));

            if (dentistExists)
            {
                ModelState.AddModelError(string.Empty, "A dentist with this email or name already exists.");
                return Page();
            }

            _context.Attach(Dentist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DentistExists(Dentist.DentistId))
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

        private bool DentistExists(int id)
        {
            return _context.Dentists.Any(e => e.DentistId == id);
        }
    }
}
