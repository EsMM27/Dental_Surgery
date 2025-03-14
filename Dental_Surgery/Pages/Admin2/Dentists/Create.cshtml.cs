using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dental.DataAccess;
using Dental.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Dental_Surgery.Pages.Admin2.Dentists
{
    public class CreateModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(Dental.DataAccess.AppDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _environment = webHostEnvironment;
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

            var file = Request.Form.Files["files"];

            if (file != null && file.Length > 0)
            {
                string uploadFolder = Path.Combine(_environment.WebRootPath, "Images", "Dentist");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadFolder, fileName);


                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                Dentist.Image = Path.Combine("Images", "Dentist", fileName).Replace("\\", "/");
            }
            else
            {
                Console.WriteLine("No file uploaded.");
            }

           

            _context.Dentists.Add(Dentist);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
