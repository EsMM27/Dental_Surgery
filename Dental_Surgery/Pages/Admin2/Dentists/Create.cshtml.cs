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
using Dental.Service;
using System.Numerics;

namespace Dental_Surgery.Pages.Admin2.Dentists
{
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;
        public IEnumerable<Dentist> Dentists;

        public CreateModel(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _environment = webHostEnvironment;
        }

        [BindProperty]
        public Dentist Dentist { get; set; } = default!;
        public void OnGet()
        {
            Dentists = _unitOfWork.DentistRepo.GetAll();
        }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPost(Dentist dentist)
        {

            if (!ModelState.IsValid)
            {
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
                    file.CopyTo(fileStream);
                }

                Dentist.Image = Path.Combine("Images", "Dentist", fileName).Replace("\\", "/");
            }
            else
            {
                //Console.WriteLine("No file uploaded.");
                Dentist.Image = "Images/Dentist/default.png";
            }

            _unitOfWork.DentistRepo.Add(Dentist);
            _unitOfWork.Save();

            return RedirectToPage("./Index");
        }

    }
}
