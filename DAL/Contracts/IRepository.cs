using DAL.Implementation.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DAL.Contracts
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly SistemaPresupuestarioContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(SistemaPresupuestarioContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task<T> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
        public async Task AddAsync(T entity) => _dbSet.Add(entity);
        public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;
        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}
