using Dental.DataAccess;
using Dental.DataAccess.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _appDBContext;

        public IAppointmentRepo appointmentRepo { get; private set; }
        public IDentistRepo dentistRepo { get; private set; }
        public IPatientRepo patientRepo { get; private set; }

        public UnitOfWork(AppDBContext appDBContext) 
        {
            _appDBContext = appDBContext;
            appointmentRepo = new AppointmentRepo(_appDBContext);
            dentistRepo = new DentistRepo(_appDBContext);
            patientRepo = new PatientRepo(_appDBContext);
        }

        public async Task Save()
        { 
        await _appDBContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _appDBContext?.Dispose();
        }


    }
}
