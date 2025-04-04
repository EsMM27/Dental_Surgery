using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.Model;
using Microsoft.AspNetCore.Authorization;

namespace Dental_Surgery.Pages.Admin2.Dentists
{
	[Authorize(Roles = "Admin")]
	public class DeleteModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public DeleteModel(Dental.DataAccess.AppDBContext context)
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

            var dentist = await _context.Dentists.FirstOrDefaultAsync(m => m.DentistId == id);

            if (dentist == null)
            {
                return NotFound();
            }
            else
            {
                Dentist = dentist;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dentist = await _context.Dentists.FindAsync(id);
            if (dentist != null)
            {
                Dentist = dentist;
                _context.Dentists.Remove(Dentist);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
