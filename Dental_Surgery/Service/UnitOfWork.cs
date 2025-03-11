using Dental.DataAccess;
using Dental.DataAccess.Repo;
using Dental.Model;
using Microsoft.EntityFrameworkCore;

namespace Dental.Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _appDBContext;

        private IRepository<Dentist, int> _dentists;
        private IRepository<Patient, string> _patients;
        private IRepository<Treatment, int> _treatments;
        //private IRepository<Appointment> _appointments;
        private IAppointmentRepo _appointments;


        public UnitOfWork(AppDBContext appDBContext, IAppointmentRepo appointmentRepository) 
        {
            _appDBContext = appDBContext;
            _appointments = appointmentRepository;
        }

        public IRepository<Dentist, int> Dentists => _dentists ??= new Repository<Dentist, int>(_appDBContext);
        public IRepository<Patient, string> Patients => _patients ??= new Repository<Patient, string>(_appDBContext);
        public IRepository<Treatment, int> Treatments => _treatments ??= new Repository<Treatment, int>(_appDBContext);
        public IAppointmentRepo Appointments => _appointments;

        public async Task SaveAsync()
        {
            await _appDBContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _appDBContext?.Dispose();
        }


    }

}
