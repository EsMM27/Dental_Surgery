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
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Dental_Surgery.Pages.Admin2.Dentists
{
    public class CreateModel : PageModel
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(AppDBContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _environment = webHostEnvironment;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Dentist Dentist { get; set; } = default!;

        [BindProperty]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Required]
        public string ConfirmPassword { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Log or inspect the validation errors
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
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

            // Create IdentityUser
            var user = new IdentityUser
            {
                UserName = Dentist.Email,
                Email = Dentist.Email
            };

            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                // Link the IdentityUser to the Dentist
                Dentist.UserId = user.Id;

                _context.Dentists.Add(Dentist);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
    }
}
