using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dental.Model;
using Dental.Service;

namespace Dental.Service.Seeders
{
    public static class TodayAppointmentSeeder
    {
        public static async Task SeedTodayAsync(IUnitOfWork unitOfWork)
        {
            var appointments = new List<Appointment>();
            var validTimes = new[]
            {
                "09:00", "09:30", "10:00", "10:30", "11:00", "11:30",
                "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00"
            };

            var rng = new Random();

            var patientIds = (await unitOfWork.Patients.GetAllAsync()).Select(p => p.PatientId).ToList();
            var dentistIds = (await unitOfWork.Dentists.GetAllAsync()).Select(d => d.DentistId).ToList();
            var treatmentIds = (await unitOfWork.Treatments.GetAllAsync()).Select(t => t.TreatmentId).ToList();

            if (!patientIds.Any() || !dentistIds.Any() || !treatmentIds.Any())
            {
                Console.WriteLine("⚠ Cannot seed today's appointments — missing patients, dentists, or treatments.");
                return;
            }

            var today = DateTime.Today;

            // Skip if today's appointments already exist
            var existingAppointments = await unitOfWork.Appointments.GetByConditionAsync(a =>
                a.AppointmentDate.Date == today);

            //if (existingAppointments.Any())
            //{
            //    Console.WriteLine("⚠ Appointments already exist for today. Skipping.");
            //    return;
            //}

            int numberOfAppointments = rng.Next(5, 10);
            var selectedTimes = validTimes.OrderBy(_ => rng.Next()).Take(numberOfAppointments);

            foreach (var time in selectedTimes)
            {
                var dateTime = today.Add(TimeSpan.Parse(time));

                appointments.Add(new Appointment
                {
                    AppointmentDate = dateTime,
                    Notes = $"Seeded for today at {time}",
                    attend = rng.NextDouble() > 0.2,
                    PatientId = 2,
                    DentistId = dentistIds[rng.Next(dentistIds.Count)],
                    TreatmentId = treatmentIds[rng.Next(treatmentIds.Count)]
                });
            }

            Console.WriteLine($"✅ Seeding {appointments.Count} appointments for today...");
            await unitOfWork.Appointments.AddRangeAsync(appointments);
            await unitOfWork.SaveAsync();
            Console.WriteLine("🎉 Today's appointments seeded successfully!");
        }
    }
}
