using Dental.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.DataAccess.Repo
{
    public interface IDentistRepo : IRepository<Dentist, int>
    {
        public void Update(Dentist dentist);
    }
}
