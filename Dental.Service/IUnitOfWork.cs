

using Dental.DataAccess.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Service
{
    public interface IUnitOfWork : IDisposable
    {
        IAppointmentRepo appointmentRepo { get; }
        IDentistRepo dentistRepo { get; }
        IPatientRepo patientRepo { get; }

        Task Save();

    }
}
