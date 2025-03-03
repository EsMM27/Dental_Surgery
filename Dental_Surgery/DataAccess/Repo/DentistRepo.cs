using Dental.Model;
using Dental.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.DataAccess.Repo
{
    public class DentistRepo : Repository<Dentist>, IDentistRepo
    {
        private readonly AppDBContext _context;
        public DentistRepo(AppDBContext context) : base(context)
        {
            _context = context;
        }

        public void SaveAll()
        {
            _context.SaveChanges();
        }
    }
}
