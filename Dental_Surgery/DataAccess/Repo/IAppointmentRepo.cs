using Dental.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.DataAccess.Repo
{
    public interface IAppointmentRepo : IRepository<Appointment>
    {
        public void Update(Appointment appointment);
        void SaveAll();
    }
}
