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

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        public async Task OnGetAsync()
        {
            var query = _context.Appointments
                .Include(a => a.Dentist)
                .Include(a => a.Patient)
                .Include(a => a.Treatment)
                .Where(a => a.AppointmentDate.Date == ScheduleDate.Date);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(a =>
                    a.Patient != null &&
                    (a.Patient.FirstName + " " + a.Patient.LastName).ToLower().Contains(SearchTerm.ToLower()));
            }

            var allAppointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            GroupedAppointments = allAppointments
                .GroupBy(a => a.AppointmentDate.Date)
                .OrderBy(g => g.Key)
                .ToList();
        }

        public DateTime PreviousWeekday => GetPreviousWeekday(ScheduleDate);

        public DateTime NextWeekday => GetNextWeekday(ScheduleDate);

        private DateTime GetPreviousWeekday(DateTime date)
        {
            do
            {
                date = date.AddDays(-1);
            } while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
            return date;
        }

        private DateTime GetNextWeekday(DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            } while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
            return date;
        }


    }
}
