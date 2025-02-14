using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;


namespace Dental.DataAccess.Repo
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDBContext _context;
        internal DbSet<T> dbSet;
        public Repository(AppDBContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public void Add(T obj)
        {
            dbSet.Add(obj);
        }
        public void Delete(T obj)
        {
            dbSet.Remove(obj);
        }
        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }
        public T? Get(int id)
        {
            if (id == 0)
            {
                return null;
            }
            else
            {
                return dbSet.Find(id);
            }
        }
        public void Update(T obj)
        {
            dbSet.Update(obj);
        }
    }
}
