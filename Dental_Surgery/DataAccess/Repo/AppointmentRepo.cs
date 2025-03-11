﻿using Dental.Model;
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
    public class AppointmentRepo : Repository<Appointment, int>, IAppointmentRepo
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
    }
}
