using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dental.Model;
using Dental.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dental_Surgery.Pages.Admin2.Appointments
{
    public class AvailableSlotsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public AvailableSlotsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<AvailableSlot> AvailableSlots { get; set; } = new();
        public List<Dentist> AllDentists { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [BindProperty(SupportsGet = true)]
        public int? DentistId { get; set; }

        public async Task OnGetAsync()
        {
            AllDentists = (await _unitOfWork.Dentists.GetAllAsync()).ToList();

            var endDate = StartDate.AddDays(7); // one week of availability

            var dentistsToCheck = DentistId.HasValue
                ? AllDentists.Where(d => d.DentistId == DentistId.Value).ToList()
                : AllDentists;

            foreach (var date in EachDay(StartDate, endDate))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var appointments = await _unitOfWork.Appointments.GetAppointmentsForDateAsync(date);

                foreach (var dentist in dentistsToCheck)
                {
                    var bookedTimes = appointments
                        .Where(a => a.DentistId == dentist.DentistId)
                        .Select(a => a.AppointmentDate.TimeOfDay)
                        .ToHashSet();

                    TimeSpan open = new TimeSpan(9, 0, 0);
                    TimeSpan close = new TimeSpan(16, 0, 0);
                    TimeSpan slot = open;

                    while (slot <= close)
                    {
                        if (slot >= new TimeSpan(11, 30, 0) && slot < new TimeSpan(13, 0, 0))
                        {
                            slot = new TimeSpan(13, 0, 0);
                            continue;
                        }

                        // Skip past time slots for today
                        if (date == DateTime.Today && slot <= DateTime.Now.TimeOfDay)
                        {
                            slot = slot.Add(TimeSpan.FromMinutes(30));
                            continue;
                        }


                        if (!bookedTimes.Contains(slot))
                        {
                            AvailableSlots.Add(new AvailableSlot
                            {
                                Date = date,
                                Time = slot,
                                DentistName = $"Dr. {dentist.LastName}",
                                DentistId = dentist.DentistId
                            });
                        }

                        slot = slot.Add(TimeSpan.FromMinutes(30));
                    }
                }
            }
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
                yield return day;
        }
    }

    public class AvailableSlot
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string DentistName { get; set; }
        public int DentistId { get; set; }
    }
}
