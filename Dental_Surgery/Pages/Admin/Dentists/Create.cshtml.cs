using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dental.DataAccess;
using Dental.Model;

namespace Dental_Surgery.Pages.Admin.Dentists
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
            return Page();
        }

        [BindProperty]
        public Dentist Dentist { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid.");

                // Log validation errors
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            _context.Dentists.Add(Dentist);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }



    }
}
