
using Dental.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Dental.DataAccess
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Dentist)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DentistId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PPS);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Treatment)
                .WithMany(t => t.Appointments)
                .HasForeignKey(a => a.TreatmentId);

            // Configure precision and scale for the Cost property
            modelBuilder.Entity<Treatment>()
                .Property(t => t.Cost)
                .HasPrecision(18, 2); // 18 digits total, 2 digits after the decimal point
        }
        public DbSet<Dentist> Dentists { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Treatment> Treatments { get; set; }

    }

}