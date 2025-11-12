using DAL.Implementation.EntityFramework;
using DomainModel.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task<T> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
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
