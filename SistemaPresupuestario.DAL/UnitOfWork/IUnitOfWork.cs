using System;
using SistemaPresupuestario.DAL.Repositories.Interfaces;

namespace SistemaPresupuestario.DAL.UnitOfWork
{
    /// <summary>
    /// Unit of Work pattern interface for managing database transactions
    /// Coordinates changes across multiple repositories and ensures consistency
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Usuario repository
        /// </summary>
        IUsuarioRepository Usuarios { get; }

        /// <summary>
        /// Familia repository
        /// </summary>
        IFamiliaRepository Familias { get; }

        /// <summary>
        /// Patente repository
        /// </summary>
        IPatenteRepository Patentes { get; }

        /// <summary>
        /// Saves all pending changes to the database
        /// </summary>
        /// <returns>Number of affected records</returns>
        int SaveChanges();

        /// <summary>
        /// Begins a database transaction
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Executes an action within a transaction
        /// Automatically handles commit on success and rollback on exception
        /// </summary>
        /// <param name="action">Action to execute</param>
        void ExecuteInTransaction(Action action);

        /// <summary>
        /// Executes a function within a transaction
        /// Automatically handles commit on success and rollback on exception
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="func">Function to execute</param>
        /// <returns>Function result</returns>
        T ExecuteInTransaction<T>(Func<T> func);
    }
}