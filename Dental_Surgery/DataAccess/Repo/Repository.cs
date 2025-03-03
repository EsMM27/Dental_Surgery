using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;


namespace Dental.DataAccess.Repo
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDBContext _context;
        private readonly DbSet<T> dbSet;

        public Repository(AppDBContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> list = dbSet;
            return list.ToList();
        }

        public T? Get(object id) //generic object to allow string(pps) and ints (appt & doctor ids)
        {
            if (id == null)
                return null;
            else
                return dbSet.Find(id);
        }

        public void Add(T obj)
        {
            dbSet.Add(obj);
        }

        public void Update(T obj)
        {
            dbSet.Update(obj);
        }

        public void Delete(T obj)
        {
            dbSet.Remove(obj);
        }
    }
}
