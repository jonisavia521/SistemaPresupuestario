using DomainModel.Domain;
using System;

namespace DomainModel.Contract
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositorios
        IClienteRepository ClienteRepository { get; }
        IVendedorRepository VendedorRepository { get; }
        IProductoRepository ProductoRepository { get; }
        IPresupuestoRepository PresupuestoRepository { get; }

        // Métodos para manejar transacciones y confirmar los cambios
        void BeginTransaction();               // Inicia una transacción

        void Commit();                         // Confirma los cambios en la transacción

        void Rollback();                       // Revertir la transacción si algo falla

        int SaveChanges();                     // Guardar los cambios sin transacción explícita
    }
}
