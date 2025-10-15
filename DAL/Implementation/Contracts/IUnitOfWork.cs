using DAL.Implementation.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        // Métodos para manejar transacciones y confirmar los cambios
        void BeginTransaction();               // Inicia una transacción

        void Commit();                         // Confirma los cambios en la transacción

        void Rollback();                       // Revertir la transacción si algo falla

        int SaveChanges();                     // Guardar los cambios sin transacción explícita
    }
}
