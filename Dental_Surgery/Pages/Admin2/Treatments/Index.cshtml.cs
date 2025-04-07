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

namespace Dental_Surgery.Pages.Admin2.Treatments
{
	[Authorize(Roles = "Admin")]
	public class IndexModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public IndexModel(Dental.DataAccess.AppDBContext context)
        {
            _context = context;
        }

        public IList<Treatment> Treatment { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Treatment = await _context.Treatments.ToListAsync();
        }
    }
}
