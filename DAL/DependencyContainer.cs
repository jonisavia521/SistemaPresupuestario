using DAL.Implementation.Repository;
using DAL.Implementation.Repository.Seguridad;
using DAL.Contracts;
using DAL.Contracts.Seguridad;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Implementation.EntityFramework;

namespace DAL
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddDALDependencies(
           this IServiceCollection services, System.Configuration.ConnectionStringSettings csSetting)
        {
            var _ = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
            // --- FIN DEL TRUCO ---
            services.AddScoped<SistemaPresupuestario>(opt => new SistemaPresupuestario(csSetting.ConnectionString));

            // Repositorios originales
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            
            // NUEVOS REPOSITORIOS DE SEGURIDAD
            // DECISIÓN: Registrar repositorios específicos para ABM de usuarios con permisos
            services.AddScoped<IUsuarioSecurityRepository, UsuarioSecurityRepository>();
            services.AddScoped<IFamiliaRepository, FamiliaRepository>();
            services.AddScoped<IPatenteRepository, PatenteRepository>();
            
            // UnitOfWork actualizado con repositorios de seguridad
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            return services;
        }
    }
}
