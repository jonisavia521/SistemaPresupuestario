using DAL.Implementation.EntityFramework;
using DomainModel.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly SistemaPresupuestario _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(SistemaPresupuestario context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll() => _dbSet.ToList();
        public T GetById(int id) => _dbSet.Find(id);
        public T GetById(Guid id) => _dbSet.Find(id);
        public void Add(T entity) => _dbSet.Add(entity);
        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }
    }
}
