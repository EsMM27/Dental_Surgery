using Dental.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.DataAccess.Repo
{
    public interface IPatientRepo : IRepository<Patient>
    {
        public void Update(Patient patient);
    }
}
