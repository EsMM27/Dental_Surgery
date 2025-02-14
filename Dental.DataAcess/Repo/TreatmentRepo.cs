using Dental.DataAccess;
using Dental.DataAccess.Repo;
using Dental.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.DataAcess.Repo
{
    public class TreatmentRepo : Repository<Treatment>, ITreatmentRepo
    {
        private readonly AppDBContext _context;
        public TreatmentRepo(AppDBContext context) : base(context)
        {
            _context = context;
        }
    }
}
