using System;
using System.Data.Entity;
using SistemaPresupuestario.DAL.Context;
using SistemaPresupuestario.DAL.Repositories.Implementations;
using SistemaPresupuestario.DAL.Repositories.Interfaces;

namespace SistemaPresupuestario.DAL.UnitOfWork
{
    /// <summary>
    /// Unit of Work implementation for managing database transactions
    /// Ensures all repository operations are coordinated and atomic
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SistemaPresupuestarioContext _context;
        private DbContextTransaction _transaction;
        private bool _disposed = false;

        // Repository instances
        private IUsuarioRepository _usuarios;
        private IFamiliaRepository _familias;
        private IPatenteRepository _patentes;

        public UnitOfWork(SistemaPresupuestarioContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UnitOfWork() : this(new SistemaPresupuestarioContext())
        {
        }

        #region Repository Properties

        public IUsuarioRepository Usuarios
        {
            get
            {
                if (_usuarios == null)
                    _usuarios = new UsuarioRepository(_context);
                return _usuarios;
            }
        }

        public IFamiliaRepository Familias
        {
            get
            {
                if (_familias == null)
                    _familias = new FamiliaRepository(_context);
                return _familias;
            }
        }

        public IPatenteRepository Patentes
        {
            get
            {
                if (_patentes == null)
                    _patentes = new PatenteRepository(_context);
                return _patentes;
            }
        }

        #endregion

        #region Transaction Management

        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception if logging is available
                // For now, just re-throw
                throw new InvalidOperationException("Error saving changes to database", ex);
            }
        }

        public void BeginTransaction()
        {
            if (_transaction != null)
                throw new InvalidOperationException("Transaction already in progress");

            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction in progress");

            try
            {
                SaveChanges();
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction in progress");

            try
            {
                _transaction.Rollback();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void ExecuteInTransaction(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            bool transactionStartedHere = _transaction == null;

            if (transactionStartedHere)
                BeginTransaction();

            try
            {
                action();

                if (transactionStartedHere)
                    CommitTransaction();
            }
            catch
            {
                if (transactionStartedHere && _transaction != null)
                    RollbackTransaction();
                throw;
            }
        }

        public T ExecuteInTransaction<T>(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            bool transactionStartedHere = _transaction == null;

            if (transactionStartedHere)
                BeginTransaction();

            try
            {
                T result = func();

                if (transactionStartedHere)
                    CommitTransaction();

                return result;
            }
            catch
            {
                if (transactionStartedHere && _transaction != null)
                    RollbackTransaction();
                throw;
            }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Rollback any pending transaction
                    if (_transaction != null)
                    {
                        _transaction.Rollback();
                        _transaction.Dispose();
                        _transaction = null;
                    }

                    // Dispose context
                    _context?.Dispose();
                }

                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        #endregion
    }
}