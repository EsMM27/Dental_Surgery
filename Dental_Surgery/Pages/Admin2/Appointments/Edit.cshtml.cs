using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.Model;
using Microsoft.AspNetCore.Authorization;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
	[Authorize(Roles = "Admin,Receptionist,Dentist")]
	public class EditModel : PageModel
    {
        private readonly Dental.DataAccess.AppDBContext _context;

        public EditModel(Dental.DataAccess.AppDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }
            Appointment = appointment;
            ViewData["DentistId"] = new SelectList(_context.Dentists
                .Select(d => new
                {
                    d.DentistId,
                    FullName = "Dr. " + d.FirstName + " " + d.LastName
                }),
                 "DentistId",
                 "FullName"
             );

            ViewData["PatientId"] = new SelectList(
                _context.Patients
                    .Select(p => new
                    {
                        p.PatientId,
                        FullName = p.FirstName + " " + p.LastName
                    }),
                "PatientId",
                "FullName"
            );
            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "TreatmentId", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(Appointment.AppointmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
