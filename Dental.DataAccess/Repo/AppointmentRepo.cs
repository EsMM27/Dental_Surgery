using Dental.Model;
using Dental.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.DataAccess.Repo
{
    public class AppointmentRepo : Repository<Appointment>, IAppointmentRepo
    {
        private readonly AppDBContext _context;
        public AppointmentRepo(AppDBContext context) : base(context)
        {
            _context = context;
        }
    }
}
