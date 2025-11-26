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
    /// 
    /// Esta clase estática proporciona métodos de extensión que registran todos los repositorios,
    /// el contexto de base de datos y configuraciones de acceso a datos en el contenedor
    /// de inyección de dependencias de Microsoft.Extensions.DependencyInjection.
    /// 
    /// Responsabilidades:
    /// - Configurar Entity Framework (EF) con SQL Server
    /// - Registrar el contexto de base de datos
    /// - Registrar todos los repositorios (patrón Repository)
    /// - Registrar la unidad de trabajo (Unit of Work pattern)
    /// 
    /// Los repositorios se registran como Scoped, lo que significa que se crea una nueva instancia
    /// por cada solicitud/scope HTTP (en web) o por cada operación en una aplicación de escritorio.
    /// El contexto de EF también es Scoped para asegurar coherencia transaccional.
    /// </summary>
    public static class DependencyContainer
    {
        /// <summary>
        /// Registra todas las dependencias de la capa DAL en el contenedor de inyección de dependencias.
        /// 
        /// Este método de extensión es responsable de:
        /// 1. Cargar el provider de Entity Framework para SQL Server (workaround para DLL no encontrada)
        /// 2. Registrar el contexto de base de datos SistemaPresupuestario
        /// 3. Registrar la unidad de trabajo (IUnitOfWork)
        /// 4. Registrar todos los repositorios específicos del dominio
        /// 
        /// Debe ser llamado desde el contenedor IoC principal (IoC.DependencyContainer) durante
        /// la inicialización de la aplicación.
        /// </summary>
        /// <param name="services">
        /// El contenedor de inyección de dependencias de Microsoft.Extensions.DependencyInjection
        /// donde se registrarán todos los repositorios y configuraciones de DAL.
        /// </param>
        /// <param name="csSetting">
        /// Objeto System.Configuration.ConnectionStringSettings que contiene los parámetros
        /// de conexión a SQL Server:
        /// - Servidor (Data Source)
        /// - Usuario (User Id)
        /// - Contraseña (Password)
        /// - Nombre de la base de datos (Initial Catalog)
        /// Se utiliza para crear el DbContext de Entity Framework.
        /// </param>
        /// <returns>
        /// El mismo contenedor de servicios (IServiceCollection) para permitir encadenamiento
        /// de llamadas (fluent API) en la configuración de dependencias.
        /// </returns>
        /// <example>
        /// <code>
        /// var csSetting = ConfigurationManager.ConnectionStrings["MiAplicacion"];
        /// services.AddDALDependencies(csSetting);
        /// </code>
        /// </example>
        public static IServiceCollection AddDALDependencies(
           this IServiceCollection services, System.Configuration.ConnectionStringSettings csSetting)
        {
            /// <summary>
            /// WORKAROUND: Forzar la carga del ensamblado EntityFramework.SqlServer.dll
            /// 
            /// PROBLEMA: En algunos entornos, Entity Framework no carga automáticamente el provider
            /// de SQL Server, resultando en el error:
            /// "No Entity Framework provider found for SQL Server"
            /// 
            /// SOLUCIÓN: Hacer una referencia explícita a System.Data.Entity.SqlServer.SqlProviderServices
            /// garantiza que el ensamblado se cargue en memoria antes de intentar usar EF.
            /// 
            /// La variable "_" indica que no usamos la instancia pero necesitamos que se cargue el tipo.
            /// Esta es una práctica común en .NET para forzar la ejecución de inicializadores estáticos.
            /// 
            /// Sin esta línea, algunos proyectos pueden fallar con:
            /// - System.InvalidOperationException
            /// - System.Reflection.ReflectionTypeLoadException
            /// - Provider not found exception
            /// </summary>
            var _ = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
       
            /// <summary>
            /// Registra el contexto de base de datos de Entity Framework como Scoped.
            /// 
            /// SistemaPresupuestario es el DbContext que:
            /// - Define los DbSets para cada entidad (Clientes, Productos, Presupuestos, etc.)
            /// - Gestiona la conexión a la base de datos SQL Server
            /// - Proporciona el unit of work implícito de EF
            /// - Realiza el mapeo de entidades a tablas de BD
            /// 
            /// Registra como Scoped significa:
            /// - Se crea UNA NUEVA instancia por cada scope (típicamente una solicitud HTTP o una operación)
            /// - Todos los repositorios dentro del mismo scope comparten el mismo contexto
            /// - Esto facilita las transacciones y la coherencia de datos
            /// - Se dispone automáticamente al final del scope
            /// 
            /// Se registra usando una factory (opt => new ...) para pasar la cadena de conexión
            /// desde la configuración al constructor del contexto.
            /// </summary>
            services.AddScoped<SistemaPresupuestario>(opt => new SistemaPresupuestario(csSetting.ConnectionString));

            /// <summary>
            /// Registra la unidad de trabajo (Unit of Work pattern) como Scoped.
            /// 
            /// IUnitOfWork proporciona:
            /// - Acceso a todos los repositorios de forma coordinada
            /// - Gestión de transacciones
            /// - Control de cambios pendientes en el contexto EF
            /// - Método SaveChanges() para persistir todos los cambios de una vez
            /// 
            /// La implementación UnitOfWork recibe el SistemaPresupuestario DbContext
            /// inyectado y lo utiliza para coordinar operaciones en múltiples repositorios.
            /// </summary>
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            /// <summary>
            /// Registra todos los repositorios específicos del dominio como Scoped.
            /// 
            /// Cada repositorio implementa el patrón Repository para una entidad:
            /// - ClienteRepository: CRUD para entidades Cliente
            /// - VendedorRepository: CRUD para entidades Vendedor
            /// - ProductoRepository: CRUD para entidades Producto
            /// - PresupuestoRepository: CRUD para entidades Presupuesto (con detalles)
            /// - ConfiguracionRepository: Acceso a configuración del sistema
            /// 
            /// Los repositorios reciben el SistemaPresupuestario DbContext inyectado
            /// y realizan operaciones de base de datos encapsuladas (desacoplando la UI de EF).
            /// 
            /// Se registran como Scoped para garantizar que todos compartan el mismo
            /// contexto durante una operación y mantener coherencia de datos.
            /// </summary>
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IVendedorRepository, VendedorRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IPresupuestoRepository, PresupuestoRepository>();
            
            /// <summary>
            /// Nota: ListaPrecioRepository se registra sin interfaz.
            /// Se registra directamente la clase concreta porque su interfaz no está definida
            /// o porque solo se utiliza internamente sin necesidad de abstracción.
            /// 
            /// En futuras refactorizaciones, se recomienda:
            /// 1. Crear IListaPrecioRepository
            /// 2. Cambiar el registro a: services.AddScoped<IListaPrecioRepository, ListaPrecioRepository>();
            /// 3. Actualizar todos los usos para depender de la interfaz, no de la clase
            /// </summary>
            services.AddScoped<ListaPrecioRepository>();
            
            /// <summary>
            /// Registra el repositorio de Configuración como Scoped.
            /// 
            /// ConfiguracionRepository proporciona acceso a los datos de configuración del sistema
            /// (parámetros globales como razón social de la empresa, idioma, etc.).
            /// </summary>
            services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
            
            return services;
        }
    }
}
