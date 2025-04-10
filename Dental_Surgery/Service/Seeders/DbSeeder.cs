// Auto-generated appointment seeder using Unit of Work pattern
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dental.Model;
using Dental.Service;

namespace Dental.Service.Seeders
{
    public static class DbSeeder
    {
        public static async Task SeedAppointmentsAsync(IUnitOfWork unitOfWork)
        {
            var appointments = new List<Appointment>();
            var validTimes = new[]
            {
        "09:00", "09:30", "10:00", "10:30", "11:00", "11:30",
        "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00"
    };

            var rng = new Random();

            var existingPatientIds = (await unitOfWork.Patients.GetAllAsync()).Select(p => p.PatientId).ToList();
            var existingDentistIds = (await unitOfWork.Dentists.GetAllAsync()).Select(d => d.DentistId).ToList();
            var existingTreatmentIds = (await unitOfWork.Treatments.GetAllAsync()).Select(t => t.TreatmentId).ToList();

            if (!existingPatientIds.Any() || !existingDentistIds.Any() || !existingTreatmentIds.Any())
            {
                Console.WriteLine(" Cannot seed appointments — missing patients, dentists, or treatments.");
                return;
            }

            for (int dayOffset = -15; dayOffset <= 15; dayOffset++)
            {
                var date = DateTime.Today.AddDays(dayOffset);
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var startOfDay = date.Date;
                var endOfDay = startOfDay.AddDays(1);

                //  Skip if appointments already exist for this date
                var existingAppointments = await unitOfWork.Appointments.GetByConditionAsync(a =>
                    a.AppointmentDate >= startOfDay && a.AppointmentDate < endOfDay);

                if (existingAppointments.Any())
                {
                    Console.WriteLine($" Skipping {date:yyyy-MM-dd} (already has appointments)");
                    continue;
                }

                //  Fully book 3 days in the future
                bool fullyBooked = dayOffset == 2 || dayOffset == 4 || dayOffset == 6;

                int slotsToFill = fullyBooked ? validTimes.Length : rng.Next(2, 5);
                var selectedTimes = validTimes.OrderBy(_ => rng.Next()).Take(slotsToFill);

                foreach (var time in selectedTimes)
                {
                    var dateTime = date.Add(TimeSpan.Parse(time));

                    appointments.Add(new Appointment
                    {
                        AppointmentDate = dateTime,
                        Notes = fullyBooked
                            ? $"Fully booked day - {dateTime:yyyy-MM-dd HH:mm}"
                            : $"Auto-generated for {dateTime:yyyy-MM-dd HH:mm}",
                        attend = rng.NextDouble() > 0.2,
                        PatientId = existingPatientIds[rng.Next(existingPatientIds.Count)],
                        DentistId = existingDentistIds[rng.Next(existingDentistIds.Count)],
                        TreatmentId = existingTreatmentIds[rng.Next(existingTreatmentIds.Count)]
                    });
                }
            }

            Console.WriteLine($" Seeding {appointments.Count} appointments...");
            await unitOfWork.Appointments.AddRangeAsync(appointments);
            await unitOfWork.SaveAsync();
            Console.WriteLine(" Appointments seeded successfully!");
        }

    }
}