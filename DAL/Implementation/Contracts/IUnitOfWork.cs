using DAL.Implementation.Repository;
using DAL.Contracts.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IUsuarioRepository Usuarios { get; }
        
        // NUEVOS REPOSITORIOS DE SEGURIDAD
        // DECISIÓN: Extensión del UnitOfWork para soporte completo de ABM de usuarios con permisos
        IUsuarioSecurityRepository UsuariosSecurity { get; }
        IFamiliaRepository Familias { get; }
        IPatenteRepository Patentes { get; }
        
        // Métodos para manejar transacciones y confirmar los cambios
        void BeginTransaction();               // Inicia una transacción

        void Commit();                         // Confirma los cambios en la transacción

        void Rollback();                       // Revertir la transacción si algo falla

        int SaveChanges();                     // Guardar los cambios sin transacción explícita
    }
}
