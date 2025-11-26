using BLL;
using DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC
{
    /// <summary>
    /// Contenedor de Inyección de Dependencias para la capa IoC (Inversion of Control).
    /// 
    /// Esta clase actúa como punto de entrada central para la configuración de todas las
    /// dependencias de la aplicación. Orquesta la registración de dependencias de todas las capas:
    /// - BLL (Lógica de Negocio)
    /// - DAL (Acceso a Datos)
    /// - Y cualquier otra capa que se agregue en el futuro
    /// 
    /// El patrón de Inyección de Dependencias permite:
    /// - Desacoplamiento entre capas
    /// - Facilidad de testing (uso de mocks)
    /// - Flexibilidad para cambiar implementaciones
    /// - Mantenibilidad y escalabilidad del código
    /// </summary>
    public static class DependencyContainer
    {
        /// <summary>
        /// Método de extensión que registra TODAS las dependencias de la aplicación
        /// en el contenedor de inyección de dependencias de Microsoft.Extensions.
        /// 
        /// IMPORTANTE: Este es el ÚNICO método que debe llamar la capa de Presentación (UI).
        /// La UI no debe conocer detalles de cómo se configuren las capas BLL o DAL.
        /// 
        /// El método orquesta la registración de dependencias en el siguiente orden:
        /// 1. Registra servicios de la capa BLL (AutoMapper, servicios de negocio)
        /// 2. Registra repositorios y contexto de BD de la capa DAL
        /// 3. Puede extenderse para otras capas (Caching, Email, Logging, etc.)
        /// 
        /// Flujo de llamadas:
        /// UI (Program.cs)
        ///   ↓
        /// IoC.DependencyContainer.AddIoCDependencies()  ← PUNTO DE ENTRADA ÚNICO
        ///   ↓
        /// BLL.DependencyContainer.AddBLLDependencies()
        ///   ├─ Registra AutoMapper
        ///   └─ Registra servicios (ClienteService, ProductoService, etc.)
        ///   ↓
        /// DAL.DependencyContainer.AddDALDependencies()
        ///   ├─ Configura Entity Framework
        ///   └─ Registra repositorios (ClienteRepository, ProductoRepository, etc.)
        /// </summary>
        /// <param name="services">
        /// El contenedor de inyección de dependencias de Microsoft.Extensions.DependencyInjection
        /// donde se registrarán todas las dependencias de la aplicación.
        /// </param>
        /// <param name="csSetting">
        /// Objeto System.Configuration.ConnectionStringSettings que contiene los parámetros
        /// de conexión a la base de datos (servidor, usuario, contraseña, nombre de BD, etc.).
        /// Se pasa a DAL para configurar Entity Framework y las cadenas de conexión.
        /// </param>
        /// <returns>
        /// El mismo contenedor de servicios (IServiceCollection) para permitir encadenamiento
        /// de llamadas (fluent API) en la configuración de dependencias desde la UI.
        /// </returns>
        /// <example>
        /// <code>
        /// // En Program.cs o Startup.cs:
        /// var services = new ServiceCollection();
        /// var csSetting = ConfigurationManager.ConnectionStrings["MiAplicacion"];
        /// 
        /// services.AddIoCDependencies(csSetting);
        /// 
        /// using (var serviceProvider = services.BuildServiceProvider())
        /// {
        ///     var clienteService = serviceProvider.GetRequiredService<IClienteService>();
        ///     var clientes = clienteService.GetAll();
        /// }
        /// </code>
        /// </example>
        public static IServiceCollection AddIoCDependencies(
            this IServiceCollection services,
            System.Configuration.ConnectionStringSettings csSetting)
        {
            /// <summary>
            /// Paso 1: Registra las dependencias de la capa BLL (Lógica de Negocio).
            /// 
            /// Esto incluye:
            /// - Configuración de AutoMapper con todos los perfiles de mapeo
            /// - Registro de todos los servicios de negocio como Scoped:
            ///   * IClienteService, IVendedorService, IProductoService, etc.
            /// 
            /// Estos servicios contienen la lógica de validación y negocio de la aplicación.
            /// </summary>
            services.AddBLLDependencies();

            /// <summary>
            /// Paso 2: Registra las dependencias de la capa DAL (Acceso a Datos).
            /// 
            /// Esto incluye:
            /// - Configuración de Entity Framework con la cadena de conexión proporcionada
            /// - Carga del provider de SQL Server (.NET)
            /// - Registro del contexto de base de datos (SistemaPresupuestario)
            /// - Registro de todos los repositorios:
            ///   * IClienteRepository, IProductoRepository, IPresupuestoRepository, etc.
            /// - Registro de la unidad de trabajo (IUnitOfWork)
            /// </summary>
            services.AddDALDependencies(csSetting);

            /// <summary>
            /// Nota: Para agregar más capas en el futuro (ej: Caching, Email, Logging, etc.),
            /// simplemente agregar aquí:
            /// 
            /// services.AddCachingDependencies();
            /// services.AddEmailDependencies();
            /// services.AddLoggingDependencies();
            /// </summary>

            return services;
        }
    }
}
