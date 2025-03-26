using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.Model;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
    public class DetailsModel : PageModel
    {
        private readonly AppDBContext _context;

        public DetailsModel(AppDBContext context)
        {
            _context = context;
        }

        public Appointment Appointment { get; set; } = default!;


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
            else
            {
                Appointment = appointment;
            }
            return Page();
        }

    }
}
