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
        /// <summary>
        /// Registra los servicios de la Lógica de Negocio en el contenedor de inyección de dependencias.
        /// 
        /// Este método de extensión es responsable de:
        /// - Configurar AutoMapper con todos los perfiles de mapeo (ClienteMappingProfile, VendedorMappingProfile, etc.)
        /// - Registrar todos los servicios de negocio (IClienteService, IVendedorService, etc.)
        /// - Registrar todas las interfaces de contrato necesarias para la capa de presentación y DAL
        /// 
        /// Debe ser llamado desde el contenedor IoC principal durante la inicialización de la aplicación,
        /// típicamente en Program.cs o Startup.cs, después de registrar las dependencias de DAL.
        /// </summary>
        /// <param name="services">
        /// El contenedor de inyección de dependencias de Microsoft.Extensions.DependencyInjection
        /// donde se registrarán todos los servicios y configuraciones de BLL.
        /// </param>
        /// <returns>
        /// El mismo contenedor de servicios (IServiceCollection) para permitir encadenamiento de llamadas
        /// (fluent API) en la configuración de dependencias.
        /// </returns>
        /// <example>
        /// <code>
        /// var services = new ServiceCollection();
        /// services.AddBLLDependencies();
        /// </code>
        /// </example>
        public static IServiceCollection AddBLLDependencies(this IServiceCollection services)
        {
            /// <summary>
            /// Registra todos los perfiles de mapeo de AutoMapper necesarios para convertir
            /// entre modelos de dominio (Domain Model) y objetos de transferencia de datos (DTOs).
            /// Los perfiles incluyen:
            /// - ClienteMappingProfile: Mapeo entre Cliente y ClienteDTO
            /// - VendedorMappingProfile: Mapeo entre Vendedor y VendedorDTO
            /// - ProductoMappingProfile: Mapeo entre Producto y ProductoDTO
            /// - PresupuestoMappingProfile: Mapeo entre Presupuesto y PresupuestoDTO
            /// - ListaPrecioMappingProfile: Mapeo entre ListaPrecio y ListaPrecioDTO
            /// - ProvinciaMappingProfile: Mapeo entre Provincia y ProvinciaDTO
            /// - ConfiguracionMappingProfile: Mapeo entre Configuracion y ConfiguracionDTO
            /// </summary>
            services.AddAutoMapper(
                typeof(ClienteMappingProfile), 
                typeof(VendedorMappingProfile), 
                typeof(ProductoMappingProfile), 
                typeof(PresupuestoMappingProfile),
                typeof(ListaPrecioMappingProfile),
                typeof(ProvinciaMappingProfile),
                typeof(ConfiguracionMappingProfile));
            
            /// <summary>
            /// Registra los servicios de negocio como Scoped en el contenedor de dependencias.
            /// Cada servicio implementa su interfaz correspondiente y se crea una nueva instancia
            /// por cada scope (típicamente una solicitud HTTP o una operación en la UI).
            /// 
            /// Servicios registrados:
            /// - IClienteService -> ClienteService: Gestión de clientes
            /// - IVendedorService -> VendedorService: Gestión de vendedores
            /// - IProductoService -> ProductoService: Gestión de productos
            /// - IPresupuestoService -> PresupuestoService: Gestión de presupuestos/cotizaciones
            /// - IListaPrecioService -> ListaPrecioService: Gestión de listas de precios
            /// - IProvinciaService -> ProvinciaService: Gestión de provincias
            /// - IConfiguracionService -> ConfiguracionService: Gestión de configuración del sistema
            /// </summary>
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
