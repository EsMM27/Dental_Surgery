using Dental.Model;
using Dental.Service;

namespace Dental.DataAccess.Repo
{
    public class PatientRepo : Repository<Patient>, IPatientRepo
    {
        private readonly AppDBContext _context;
        public PatientRepo(AppDBContext context) : base(context)
        {
            _context = context;
        }
        public void SaveAll()
        {
            _context.SaveChanges();
        }

    }
}
