using Dental.DataAccess.Repo;
using Dental.Model;

namespace Dental.Service
{
    public interface IPatientRepo : IRepository<Patient>
    {
        public void Update(Patient patient);
    }
}