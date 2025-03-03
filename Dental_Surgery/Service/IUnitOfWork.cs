

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
        IDentistRepo DentistRepo { get; }
        IPatientRepo PatientRepo { get; }
        ITreatmentRepo TreatmentRepo { get; }
        IAppointmentRepo AppointmentRepo { get; }
        void Save();
    }
}
