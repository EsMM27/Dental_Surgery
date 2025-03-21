using Dental.DataAccess;
using Dental.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Dental_Surgery.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _context;

        public IndexModel(AppDBContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime ScheduleDate { get; set; } = DateTime.Today;

        public List<IGrouping<DateTime, Appointment>> GroupedAppointments { get; set; } = new();

        public async Task OnGetAsync()
        {
            var allAppointments = await _context.Appointments
                .Include(a => a.Dentist)
                .Include(a => a.Patient)
                .Include(a => a.Treatment)
                .Where(a => a.AppointmentDate >= ScheduleDate)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            GroupedAppointments = allAppointments
                .GroupBy(a => a.AppointmentDate.Date)
                .OrderBy(g => g.Key)
                .ToList();
        }
    }
}
