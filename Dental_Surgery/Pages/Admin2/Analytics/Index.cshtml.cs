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
            // Sample labels for different time ranges
            TimeRangeLabels = new Dictionary<string, List<string>>
            {
                { "Weekly", new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri" } },
                { "30 Day", Enumerable.Range(1, 30).Select(i => $"Day {i}").ToList() }
            };

            // Sample data for different time ranges
            TimeRangeData = new Dictionary<string, List<int>>
            {
                { "Weekly", await GetAppointmentsCountForLastWeekAsync() },
                { "30 Day", await GetAppointmentsCountForLast30DaysAsync() }
            };

            // Get today's appointments
            TodaysAppointments = await GetTodaysAppointmentsWithTreatmentAsync();
            // Get upcoming appointments count
            UpcomingAppointmentsCount = await GetUpcomingAppointmentsCountAsync();
            // Get top treatments
            TopTreatments = await GetTopTreatmentsAsync();
            // Get total number of patients
            TotalPatientsCount = await GetTotalPatientsCountAsync();
            TotalMoneyEarnedThisMonth = await GetTotalMoneyEarnedThisMonthAsync();
            TotalMoneyEarnedPreviousMonth = await GetTotalMoneyEarnedPreviousMonthAsync();

            Dictionary<string, int> treatmentCounts;
            if (DefaultTimeRange == "Weekly")
            {
                treatmentCounts = await GetTreatmentCountsForLastWeekAsync();
            }
            else
            {
                treatmentCounts = await GetTreatmentCountsForLast30DaysAsync();
            }

            TreatmentsLabels = treatmentCounts.Keys.ToList();
            TreatmentsData = treatmentCounts.Values.ToList();
        }


        public async Task<List<(DateTime AppointmentTime, string TreatmentName)>> GetTodaysAppointmentsWithTreatmentAsync()
        {
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
            var today = DateTime.Today;

            var todaysAppointments = allAppointments
                .Where(a => a.AppointmentDate.Date == today)
                .OrderBy(a => a.AppointmentDate.TimeOfDay) // Order by time
                .Select(a => (a.AppointmentDate, a.Treatment != null ? a.Treatment.Name : "No Treatment"))
                .ToList();

            return todaysAppointments;
        }

        public async Task<int> GetUpcomingAppointmentsCountAsync()
        {
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
            var now = DateTime.Now;

            var upcomingAppointmentsCount = allAppointments
                .Count(a => a.AppointmentDate > now);

            return upcomingAppointmentsCount;
        }

        public async Task<List<string>> GetTopTreatmentsAsync()
        {
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();

            var topTreatments = allAppointments
                .Where(a => a.Treatment != null)
                .GroupBy(a => a.Treatment.Name)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();

            return topTreatments;
        }

        public async Task<int> GetTotalPatientsCountAsync()
        {
            var allPatients = await _unitOfWork.Patients.GetAllAsync();
            return allPatients.Count();
        }

        public async Task<decimal> GetTotalMoneyEarnedThisMonthAsync()
        {
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var totalMoneyEarnedThisMonth = allAppointments
                .Where(a => a.AppointmentDate >= startOfMonth && a.AppointmentDate <= endOfMonth && a.Treatment != null)
                .Sum(a => a.Treatment.Cost);

            return totalMoneyEarnedThisMonth;
        }

        public async Task<decimal> GetTotalMoneyEarnedPreviousMonthAsync()
        {
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
            var startOfPreviousMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            var endOfPreviousMonth = startOfPreviousMonth.AddMonths(1).AddDays(-1);

            var totalMoneyEarnedPreviousMonth = allAppointments
                .Where(a => a.AppointmentDate >= startOfPreviousMonth && a.AppointmentDate <= endOfPreviousMonth && a.Treatment != null)
                .Sum(a => a.Treatment.Cost);

            return totalMoneyEarnedPreviousMonth;
        }

        public async Task<List<int>> GetAppointmentsCountForLast30DaysAsync()
        {
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
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

        public async Task<List<int>> GetAppointmentsCountForLastWeekAsync()
        {
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
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

        public async Task<Dictionary<string, int>> GetTreatmentCountsForLastWeekAsync()
        {
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
            var today = DateTime.Today;
            var startDate = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

            var treatmentCounts = allAppointments
                .Where(a => a.AppointmentDate.Date >= startDate && a.AppointmentDate.Date <= startDate.AddDays(4))
                .GroupBy(a => a.Treatment.Name)
                .ToDictionary(g => g.Key, g => g.Count());

            return treatmentCounts;
        }

        public async Task<Dictionary<string, int>> GetTreatmentCountsForLast30DaysAsync()
        {
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
            var today = DateTime.Today.AddDays(1);
            var startDate = today.AddDays(-29);

            var treatmentCounts = allAppointments
                .Where(a => a.AppointmentDate.Date >= startDate && a.AppointmentDate.Date <= today)
                .GroupBy(a => a.Treatment.Name)
                .ToDictionary(g => g.Key, g => g.Count());

            return treatmentCounts;
        }



    }
}
