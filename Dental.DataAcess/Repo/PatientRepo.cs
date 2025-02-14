using Dental.Model;
using Dental.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.DataAccess.Repo
{
    public class PatientRepo : Repository<Patient>, IPatientRepo
    {
        private readonly AppDBContext _context;
        public PatientRepo(AppDBContext context) : base(context)
        {
            _context = context;
        }
    }
}
