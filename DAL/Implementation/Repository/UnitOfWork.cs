using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using System;
using System.Data.Entity; // <- EF6
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace DAL.Implementation.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SistemaPresupuestario _context;
        private DbContextTransaction _transaction;

        public IUsuarioRepository Usuarios { get; }

        public UnitOfWork(SistemaPresupuestario context, IUsuarioRepository usuarioRepository)
        {
            _context = context;
            Usuarios = usuarioRepository;
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction(); // Devuelve DbContextTransaction
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

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
