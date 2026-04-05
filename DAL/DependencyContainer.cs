using DAL.Implementation.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Implementation.EntityFramework;
using DomainModel.Contract;

namespace DAL
{
    /// <summary>
    /// Contenedor de inyección de dependencias para la capa de Acceso a Datos (DAL).
    /// Esta clase estática proporciona métodos de extensión que registran todos los repositorios,
    /// el contexto de base de datos y configuraciones de acceso a datos en el contenedor
    /// </summary>
    public static class DependencyContainer
    {
        public static IServiceCollection AddDALDependencies(
           this IServiceCollection services, System.Configuration.ConnectionStringSettings csSetting)
        {
       
            var _ = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
      
            services.AddScoped<SistemaPresupuestario>(opt => new SistemaPresupuestario(csSetting.ConnectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IVendedorRepository, VendedorRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IPresupuestoRepository, PresupuestoRepository>();
           
            services.AddScoped<ListaPrecioRepository>();
            
            services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
            
            return services;
        }
    }
}
