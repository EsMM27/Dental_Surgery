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
        public DbSet<Dentist> Categories { get; set; }

        public DbSet<Appointment> Products { get; set; }

        public DbSet<Patient> Items { get; set; }

    }

}