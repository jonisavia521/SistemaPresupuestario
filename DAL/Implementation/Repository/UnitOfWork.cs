using DAL.Implementation.EntityFramework.Context;
using DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DAL.Implementation.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly SistemaPresupuestarioContext _context;
        private DbContextTransaction _transaction;
        public IUsuarioRepository Usuarios { get; }

        public UnitOfWork(SistemaPresupuestarioContext context, IUsuarioRepository usuarioRepository)
        {
            this._context = context;
            this.Usuarios = usuarioRepository;
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
