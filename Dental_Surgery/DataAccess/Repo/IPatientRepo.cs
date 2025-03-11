using Dental.DataAccess.Repo;
using Dental.Model;

namespace Dental.Service
{
    public interface IPatientRepo : IRepository<Patient, string>
    {
        public void Update(Patient patient);
    }
}