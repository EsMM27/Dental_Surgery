using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.Model;

namespace Dental_Surgery.Pages.Admin2.Dentists
{
    public class DetailsModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public DetailsModel(Dental.DataAccess.AppDBContext context)
        {
            _context = context;
        }

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
    }
}
