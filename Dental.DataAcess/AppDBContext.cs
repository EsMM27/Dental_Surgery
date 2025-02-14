
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
        public DbSet<Dentist> Dentists { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Patient> Patients { get; set; }

    }

}