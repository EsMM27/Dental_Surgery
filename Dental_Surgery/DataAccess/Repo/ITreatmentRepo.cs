using Dental.DataAccess.Repo;
using Dental.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.DataAcess.Repo
{
    public interface ITreatmentRepo : IRepository<Treatment, int>
    {
        public void Update(Treatment treatment);
    }
}
