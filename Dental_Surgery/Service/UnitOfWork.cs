using Dental.DataAccess;
using Dental.DataAccess.Repo;
using Dental.DataAcess.Repo;
using Dental.Model;
using Microsoft.EntityFrameworkCore;

namespace Dental.Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _appDBContext;

        public IDentistRepo DentistRepo { get; private set; }
        public IPatientRepo PatientRepo { get; private set; }
        public IAppointmentRepo AppointmentRepo { get; private set; }
        public ITreatmentRepo TreatmentRepo { get; private set; }


        public UnitOfWork(AppDBContext appDBContext) 
        {
            _appDBContext = appDBContext;
            DentistRepo = new DentistRepo(appDBContext);
            PatientRepo = new PatientRepo(appDBContext);
            AppointmentRepo = new AppointmentRepo(appDBContext);
            TreatmentRepo = new TreatmentRepo(appDBContext);

        }

        //public IRepository<Dentist> Dentists => _dentists ??= new Repository<Dentist>(_appDBContext);
        //public IRepository<Patient> Patients => _patients ??= new Repository<Patient>(_appDBContext);
        //public IRepository<Treatment> Treatments => _treatments ??= new Repository<Treatment>(_appDBContext);
        //public IRepository<Appointment> Appointments => _appointments ??= new Repository<Appointment>(_appDBContext);

        public void Save()
        {
            _appDBContext.SaveChanges();
        }

        public void Dispose()
        {
            _appDBContext?.Dispose();
        }


    }

}
