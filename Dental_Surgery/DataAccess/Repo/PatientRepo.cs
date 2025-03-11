using Dental.Model;
using Dental.Service;

namespace Dental.DataAccess.Repo
{
    public class PatientRepo : Repository<Patient, string>, IPatientRepo
    {
        private readonly AppDBContext _context;
        public PatientRepo(AppDBContext context) : base(context)
        {
            _context = context;
        }
    }
}
