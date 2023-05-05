using Microsoft.EntityFrameworkCore;

namespace BikeappAPI.Repository
{

    public abstract class RepositoryBase<T> where T : class
    {
        protected DbContext _context;

        public RepositoryBase(DbContext context)
        {
            _context = context;
        }

        public virtual IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public virtual T GetById(int id)
        {
            var entity = _context.Set<T>().Find(id);

            //TODO?: handle null
            return entity;
        }

        public virtual void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public virtual void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            _context.Set<T>().Remove(entity);
        }
    }

}
