using Dental.Model;
using Dental.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dental_Surgery.Utilities;

namespace Dental.DataAccess.Repo
{
    public class AppointmentRepo : Repository<Appointment>, IAppointmentRepo
    {
        private readonly AppDBContext _context;
        public AppointmentRepo(AppDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<string>> GetBookedTimeSlotsAsync(int dentistId, DateTime date)
        {
            var bookedSlots = await _context.Appointments
                .Where(a => a.DentistId == dentistId && a.AppointmentDate.Date == date.Date)
                .Select(a => a.AppointmentDate.ToString("HH:mm"))
                .ToListAsync();

            return bookedSlots;
        }

        public async Task<List<(string TimeSlot, bool IsBooked)>> GetTimeSlotsWithAvailabilityAsync(int dentistId, DateTime date)
        {
            var bookedSlots = await GetBookedTimeSlotsAsync(dentistId, date);

            var timeSlotsWithAvailability = TimeSlots.AllTimeSlots
                .Select(slot => (TimeSlot: slot, IsBooked: bookedSlots.Contains(slot)))
                .ToList();

            return timeSlotsWithAvailability;
        }
		public async Task<IEnumerable<Appointment>> GetAppointmentsForDentistAsync(int dentistId, DateTime date)
		{
			return await _context.Appointments
				.Where(a => a.DentistId == dentistId && a.AppointmentDate.Date == date.Date)
                .Include(a => a.Patient)
				.ToListAsync();
		}
        public async Task<IEnumerable<Appointment>> GetAppointmentsForDateAsync(DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.AppointmentDate.Date == date.Date)
                .Include(p => p.Patient)
                .Include(d => d.Dentist)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                                 .Include(a => a.Treatment) // Ensure Treatment is loaded
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentHistoryForDentistAsync(int dentistId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Treatment)
                .Where(a => a.DentistId == dentistId && a.AppointmentDate < DateTime.Now)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }




    }
}
