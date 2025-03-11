

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
        IRepository<Dentist, int> Dentists { get; }
        IRepository<Patient, string> Patients { get; }
        IRepository<Treatment, int> Treatments { get; }
        IAppointmentRepo Appointments { get; }
        Task SaveAsync();
    }
}
