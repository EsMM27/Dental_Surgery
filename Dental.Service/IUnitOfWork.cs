

using Dental.DataAccess.Repo;
using Dental.DataAcess.Repo;
using Dental.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Service
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Dentist> Dentists { get; }
        IRepository<Patient> Patients { get; }
        IRepository<Treatment> Treatments { get; }
        IRepository<Appointment> Appointments { get; }
        Task<int> SaveAsync();
    }
}
