using Dental.Model;
using Dental.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dental_Surgery.Pages.Admin2.Analytics
{
	[Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Dictionary<string, List<string>> TimeRangeLabels { get; set; }
        public Dictionary<string, List<int>> TimeRangeData { get; set; }
        public string DefaultTimeRange { get; set; } = "Weekly";
        public int UpcomingAppointmentsCount { get; set; }
        public List<(DateTime AppointmentTime, string TreatmentName)> TodaysAppointments { get; set; }
        public List<string> TopTreatments { get; set; }
        public int TotalPatientsCount { get; set; }
        public decimal TotalMoneyEarnedThisMonth { get; set; }
        public decimal TotalMoneyEarnedPreviousMonth { get; set; }
        public List<int> TreatmentsData { get; set; }
        public List<string> TreatmentsLabels { get; set; }

        public async Task OnGetAsync()
        {
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
            var allPatients = await _unitOfWork.Patients.GetAllAsync();

            // Sample labels for different time ranges
            TimeRangeLabels = new Dictionary<string, List<string>>
            {
                { "Weekly", new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri" } },
                { "30 Day", Enumerable.Range(1, 30).Select(i => $"Day {i}").ToList() }
            };

            // Sample data for different time ranges
            TimeRangeData = new Dictionary<string, List<int>>
            {
                { "Weekly", GetAppointmentsCountForLastWeek(allAppointments) },
                { "30 Day", GetAppointmentsCountForLast30Days(allAppointments) }
            };

            // Get today's appointments
            TodaysAppointments = GetTodaysAppointmentsWithTreatment(allAppointments);
            // Get upcoming appointments count
            UpcomingAppointmentsCount = GetUpcomingAppointmentsCount(allAppointments);
            // Get top treatments
            TopTreatments = GetTopTreatments(allAppointments);
            // Get total number of patients
            TotalPatientsCount = allPatients.Count();
            TotalMoneyEarnedThisMonth = GetTotalMoneyEarnedThisMonth(allAppointments);
            TotalMoneyEarnedPreviousMonth = GetTotalMoneyEarnedPreviousMonth(allAppointments);

            Dictionary<string, int> treatmentCounts;
            if (DefaultTimeRange == "Weekly")
            {
                treatmentCounts = GetTreatmentCountsForLastWeek(allAppointments);
            }
            else
            {
                treatmentCounts = GetTreatmentCountsForLast30Days(allAppointments);
            }

            TreatmentsLabels = treatmentCounts.Keys.ToList();
            TreatmentsData = treatmentCounts.Values.ToList();
        }

        private List<(DateTime AppointmentTime, string TreatmentName)> GetTodaysAppointmentsWithTreatment(IEnumerable<Appointment> allAppointments)
        {
            var today = DateTime.Today;

            return allAppointments
                .Where(a => a.AppointmentDate.Date == today)
                .OrderBy(a => a.AppointmentDate.TimeOfDay) // Order by time
                .Select(a => (a.AppointmentDate, a.Treatment != null ? a.Treatment.Name : "No Treatment"))
                .ToList();
        }

        private int GetUpcomingAppointmentsCount(IEnumerable<Appointment> allAppointments)
        {
            var now = DateTime.Now;

            return allAppointments.Count(a => a.AppointmentDate > now);
        }

        private List<string> GetTopTreatments(IEnumerable<Appointment> allAppointments)
        {
            return allAppointments
                .Where(a => a.Treatment != null)
                .GroupBy(a => a.Treatment.Name)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();
        }

        private decimal GetTotalMoneyEarnedThisMonth(IEnumerable<Appointment> allAppointments)
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return allAppointments
                .Where(a => a.AppointmentDate >= startOfMonth && a.AppointmentDate <= endOfMonth && a.Treatment != null)
                .Sum(a => a.Treatment.Cost);
        }

        private decimal GetTotalMoneyEarnedPreviousMonth(IEnumerable<Appointment> allAppointments)
        {
            var startOfPreviousMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            var endOfPreviousMonth = startOfPreviousMonth.AddMonths(1).AddDays(-1);

            return allAppointments
                .Where(a => a.AppointmentDate >= startOfPreviousMonth && a.AppointmentDate <= endOfPreviousMonth && a.Treatment != null)
                .Sum(a => a.Treatment.Cost);
        }

        private List<int> GetAppointmentsCountForLast30Days(IEnumerable<Appointment> allAppointments)
        {
            var today = DateTime.Today.AddDays(1);
            var startDate = today.AddDays(-29);

            var appointmentsCount = allAppointments
                .Where(a => a.AppointmentDate.Date >= startDate && a.AppointmentDate.Date <= today)
                .GroupBy(a => a.AppointmentDate.Date)
                .OrderBy(g => g.Key)
                .ToList();

            // Ensure the list has 30 entries
            var result = new List<int>(new int[30]);
            foreach (var group in appointmentsCount)
            {
                var index = (group.Key - startDate).Days;
                result[index] = group.Count();
            }

            return result;
        }

        private List<int> GetAppointmentsCountForLastWeek(IEnumerable<Appointment> allAppointments)
        {
            var today = DateTime.Today;
            var startDate = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

            var appointmentsCount = allAppointments
                .Where(a => a.AppointmentDate.Date >= startDate && a.AppointmentDate.Date <= startDate.AddDays(4))
                .GroupBy(a => a.AppointmentDate.Date)
                .OrderBy(g => g.Key)
                .ToList();

            // Ensure the list has 5 entries (Mon-Fri)
            var result = new List<int>(new int[5]);
            foreach (var group in appointmentsCount)
            {
                var index = (group.Key - startDate).Days;
                if (index >= 0 && index < 5)
                {
                    result[index] = group.Count();
                }
            }

            return result;
        }

        private Dictionary<string, int> GetTreatmentCountsForLastWeek(IEnumerable<Appointment> allAppointments)
        {
            var today = DateTime.Today;
            var startDate = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

            return allAppointments
                .Where(a => a.AppointmentDate.Date >= startDate && a.AppointmentDate.Date <= startDate.AddDays(4))
                .GroupBy(a => a.Treatment.Name)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private Dictionary<string, int> GetTreatmentCountsForLast30Days(IEnumerable<Appointment> allAppointments)
        {
            var today = DateTime.Today;
            var startDate = today.AddDays(-30);

            return allAppointments
                .Where(a => a.AppointmentDate.Date >= startDate && a.AppointmentDate.Date <= today)
                .GroupBy(a => a.Treatment.Name)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
