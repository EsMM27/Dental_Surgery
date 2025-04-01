using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.Model;
using Microsoft.AspNetCore.Identity;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
    public class DetailsModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(Dental.DataAccess.AppDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Appointment Appointment { get; set; } = default!;
        public string BackUrl { get; set; } = "/Admin2/Appointments/Index"; // Default

        public List<string> UserRoles { get; set; } = new();


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Dentist)
                .Include(a => a.Patient)
                .Include(a => a.Treatment)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            Appointment = appointment;

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserRoles = roles.ToList();

                if (roles.Contains("Receptionist"))
                {
                    BackUrl = "/Index";
                }
                else if (roles.Contains("Dentist"))
                {
                    BackUrl = "/Shared/Schedule";
                }
            }

            return Page();
        }
    }
}
