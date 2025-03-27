using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dental.Model;
using Dental.Service;
using Dental_Surgery.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Dental_Surgery.Pages.Receptionist
{
    public class IndexModel : PageModel
    {
        [Authorize(Roles = "Receptionist")]

        public IActionResult OnGet()
        {
            return RedirectToPage("/Shared/Schedule");
        }
    }

        
}