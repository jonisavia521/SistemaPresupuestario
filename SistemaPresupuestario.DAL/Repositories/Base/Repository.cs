using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using SistemaPresupuestario.DAL.Context;

namespace SistemaPresupuestario.DAL.Repositories.Base
{
    /// <summary>
    /// Generic repository implementation providing common CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly SistemaPresupuestarioContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(SistemaPresupuestarioContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public virtual T GetById(Guid id)
        {
            return _dbSet.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual IQueryable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }

            return query;
        }

        public virtual T GetFirst(Expression<Func<T, bool>> filter, string includeProperties = "")
        {
            return Get(filter, null, includeProperties).First();
        }

        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> filter, string includeProperties = "")
        {
            return Get(filter, null, includeProperties).FirstOrDefault();
        }

        public virtual bool Any(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Any(filter);
        }

        public virtual int Count(Expression<Func<T, bool>> filter = null)
        {
            return filter == null ? _dbSet.Count() : _dbSet.Count(filter);
        }

        public virtual void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.AddRange(entities);
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Remove(T entity)
        {
            if (entity == null)
                return;

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public virtual void Remove(Guid id)
        {
            T entityToDelete = _dbSet.Find(id);
            Remove(entityToDelete);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                return;

            _dbSet.RemoveRange(entities);
        }

        public virtual IEnumerable<T> GetPaged(
            int pageNumber,
            int pageSize,
            out int totalCount,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 1)
                pageSize = 10;

            var query = Get(filter, orderBy, includeProperties);
            totalCount = query.Count();

            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}