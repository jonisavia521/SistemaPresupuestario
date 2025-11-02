using DAL.Implementation.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Implementation.EntityFramework;
using DomainModel.Contracts;
using DomainModel.Contract;

namespace DAL
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddDALDependencies(
           this IServiceCollection services, System.Configuration.ConnectionStringSettings csSetting)
        {
            // Truco para forzar la carga del ensamblado EntityFramework.SqlServer.dll
            // Previene el error: "No Entity Framework provider found for SQL Server"
            // La referencia explícita garantiza que el provider esté disponible en tiempo de ejecución
            var _ = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
       
            services.AddScoped<SistemaPresupuestario>(opt => new SistemaPresupuestario(csSetting.ConnectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            // Registrar el repositorio de Cliente
            services.AddScoped<IClienteRepository, ClienteRepository>();
            
            return services;
        }
    }
}
