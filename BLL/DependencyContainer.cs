using AutoMapper;
using BLL.Contracts;
using BLL.Mappers;
using BLL.Services;
using DomainModel.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /// <summary>
    /// Contenedor de inyección de dependencias para la capa de Lógica de Negocio (BLL).
    /// 
    /// Esta clase estática proporciona métodos de extensión que registran todos los servicios,
    /// mapeos de AutoMapper y configuraciones necesarias de la capa BLL en el contenedor
    /// de inyección de dependencias de Microsoft.Extensions.DependencyInjection.
    /// 
    /// Los servicios se registran como Scoped, lo que significa que se crea una nueva instancia
    /// por cada solicitud/scope HTTP en una aplicación web o por cada operación en una aplicación de escritorio.
    /// </summary>
    public static class DependencyContainer
    {
        
        public static IServiceCollection AddBLLDependencies(this IServiceCollection services)
        {
           
            services.AddAutoMapper(
                typeof(ClienteMappingProfile), 
                typeof(VendedorMappingProfile), 
                typeof(ProductoMappingProfile), 
                typeof(PresupuestoMappingProfile),
                typeof(ListaPrecioMappingProfile),
                typeof(ProvinciaMappingProfile),
                typeof(ConfiguracionMappingProfile));
            
            
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IVendedorService, VendedorService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<IPresupuestoService, PresupuestoService>();
            services.AddScoped<IListaPrecioService, ListaPrecioService>();
            services.AddScoped<IProvinciaService, ProvinciaService>();
            services.AddScoped<IConfiguracionService, ConfiguracionService>();
            
            return services;
        }
    }
}
