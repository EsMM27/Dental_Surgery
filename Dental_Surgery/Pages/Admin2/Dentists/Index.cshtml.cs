﻿using System;
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
    public class IndexModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public IndexModel(Dental.DataAccess.AppDBContext context)
        {
            _context = context;
        }

        public IList<Dentist> Dentist { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Dentist = await _context.Dentists.ToListAsync();
        }
    }
}
