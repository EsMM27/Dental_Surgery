using Dental.DataAccess;
using Dental.DataAccess.Repo;
using Dental.Model;
using Microsoft.EntityFrameworkCore;

namespace Dental.Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _appDBContext;

        private IRepository<Dentist> _dentists;
        private IRepository<Patient> _patients;
        private IRepository<Treatment> _treatments;
        //private IRepository<Appointment> _appointments;
        private IAppointmentRepo _appointments;


        public UnitOfWork(AppDBContext appDBContext, IAppointmentRepo appointmentRepository) 
        {
            _appDBContext = appDBContext;
            _appointments = appointmentRepository;
        }

        public IRepository<Dentist> Dentists => _dentists ??= new Repository<Dentist>(_appDBContext);
        public IRepository<Patient> Patients => _patients ??= new Repository<Patient>(_appDBContext);
        public IRepository<Treatment> Treatments => _treatments ??= new Repository<Treatment>(_appDBContext);
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
