using DAL.Implementation.Repository;
using DAL.Contracts;
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

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IFamiliaRepository, FamiliaRepository>();
            services.AddScoped<IPatenteRepository, PatenteRepository>();
            
            return services;
        }
    }
}
