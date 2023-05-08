using Microsoft.EntityFrameworkCore;

namespace BikeappAPI.Repository
{

    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected DbContext context;

        public RepositoryBase(DbContext context)
        {
            this.context = context;
        }

        public virtual IQueryable<T> GetAll()
        {
            return context.Set<T>();
        }

        public virtual T GetById(int id)
        {
            var entity = context.Set<T>().Find(id);

            //TODO?: handle null
            return entity;
        }

        public virtual void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public virtual void Update(T entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            context.Set<T>().Remove(entity);
        }
    }

}
