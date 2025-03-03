using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.DataAccess.Repo
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(object id);  //object instead of explicitly an int or string
                           //patient uses a string, appt/dr uses an int
        void Add(T obj);
        void Update(T obj);
        void Delete(T obj);
    }
}
