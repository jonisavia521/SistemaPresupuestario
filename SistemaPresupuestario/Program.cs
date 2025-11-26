using BLL;
using BLL.Contracts;
using IoC;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.DAL.Tools;
using Services.Services;
using SistemaPresupuestario.Configuracion;
using SistemaPresupuestario.Helpers;
using SistemaPresupuestario.Maestros;
using SistemaPresupuestario.Maestros.Clientes;
using SistemaPresupuestario.Maestros.ListaPrecio;
using SistemaPresupuestario.Maestros.Productos;
using SistemaPresupuestario.Maestros.Vendedores;
using SistemaPresupuestario.Presupuesto;
using SistemaPresupuestario.Venta.Arba;
using SistemaPresupuestario.Venta.Factura;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario
{
    /// <summary>
    /// Contenedor estático que agrupa todos los métodos de inicialización y configuración de la aplicación.
    /// 
    /// Esta clase es responsable del bootstrapping (arranque) de la aplicación Windows Forms,
    /// incluyendo:
    /// - Inicialización de estilos visuales
    /// - Carga del sistema de inyección de dependencias
    /// - Inicialización del sistema de traducción/internacionalización
    /// - Gestión del ciclo de vida de la aplicación
    /// 
    /// Patrón: Static entry point con métodos estáticos.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal (entrypoint) para la aplicación Windows Forms.
        /// 
        /// Este método es llamado automáticamente por Windows cuando se inicia la aplicación.
        /// El atributo [STAThread] indica que la aplicación utilizará Single-Threaded Apartment,
        /// el modelo estándar para aplicaciones Windows Forms y COM.
        /// 
        /// Flujo de ejecución:
        /// 1. Habilitar estilos visuales (apariencia moderna de controles)
        /// 2. Configurar renderizado de texto compatible
        /// 3. Inicializar sistema de ayuda (CHM)
        /// 4. Crear contenedor de inyección de dependencias
        /// 5. Registrar todas las dependencias de la aplicación
        /// 6. Construir el proveedor de servicios
        /// 7. Inicializar sistema de traducción (basado en configuración DB)
        /// 8. Mostrar formulario de login
        /// 9. Si login OK: mostrar formulario principal
        ///    Si login fallido: salir de la aplicación
        /// 
        /// EXCEPCIONES:
        /// - Si no se encuentra la connection string en App.config: InvalidOperationException
        /// - Si hay errores de base de datos: Se mostrarán mensajes de error al usuario
        /// </summary>
        [STAThread]
        static void Main()
        {
            /// <summary>
            /// Habilita estilos visuales modernos para los controles Windows Forms.
            /// Sin esto, los controles se renderizan con el estilo clásico Windows XP.
            /// </summary>
            Application.EnableVisualStyles();
            
            /// <summary>
            /// Configura el renderizado de texto como compatible con versiones anteriores.
            /// Asegura consistencia en la renderización de fuentes entre versiones de .NET.
            /// </summary>
            Application.SetCompatibleTextRenderingDefault(false);
            
            /// <summary>
            /// Inicializa el sistema de ayuda (CHM - Compiled HTML Help).
            /// Carga los archivos de documentación si están disponibles,
            /// permitiendo acceso a ayuda contextual desde los formularios.
            /// </summary>
            HelpManager.Initialize();
            
            /// <summary>
            /// Crea un nuevo contenedor de inyección de dependencias.
            /// Este contenedor será poblado con todas las dependencias en el método InjectionServices.
            /// </summary>
            var services = new ServiceCollection();
            
            /// <summary>
            /// Registra todas las dependencias en el contenedor.
            /// Esto incluye:
            /// - Capas de lógica de negocio (BLL)
            /// - Capas de acceso a datos (DAL)
            /// - Servicios heredados
            /// - Todos los formularios
            /// </summary>
            InjectionServices(services);
            
            /// <summary>
            /// Construye el proveedor de servicios a partir del contenedor.
            /// Se usa "using" para asegurar que se disponga correctamente al cerrar la aplicación.
            /// El proveedor es responsable de instanciar y inyectar dependencias cuando se solicitan.
            /// </summary>
            using (var serviceProvider = services.BuildServiceProvider())
            {
                /// <summary>
                /// Inicializa el sistema de traducción según el idioma configurado en la BD.
                /// Carga los textos de los controles desde el archivo "Textos_Controles_UI.txt".
                /// Si hay errores, utiliza idioma por defecto (es-AR).
                /// </summary>
                InicializarIdioma(serviceProvider);
                
                /// <summary>
                /// Obtiene el formulario de login del contenedor de dependencias.
                /// El contenedor automáticamente inyecta todas sus dependencias.
                /// Muestra el formulario como diálogo modal.
                /// </summary>
                var formLogin = serviceProvider.GetRequiredService<frmLogin>();
                
                /// <summary>
                /// Verifica el resultado del login.
                /// - DialogResult.OK (LOGIN EXITOSO):
                ///   * Se obtiene el formulario principal (frmMain)
                ///   * Se ejecuta como aplicación principal (Application.Run)
                ///   * La aplicación se mantiene abierta hasta que el usuario cierra el formulario
                /// 
                /// - DialogResult.CANCEL (LOGIN FALLIDO):
                ///   * Se sale de la aplicación inmediatamente
                ///   * No se muestra el formulario principal
                /// </summary>
                if (formLogin.ShowDialog() == DialogResult.OK)
                {
                    /// <summary>
                    /// Login exitoso: Mostrar formulario principal.
                    /// GetRequiredService lanza excepción si el servicio no está registrado.
                    /// Application.Run bloquea hasta que el formulario se cierre.
                    /// </summary>
                    var form = serviceProvider.GetRequiredService<frmMain>();
                    Application.Run(form);
                }
                else
                {
                    /// <summary>
                    /// Login fallido o cancelado: Salir de la aplicación.
                    /// </summary>
                    Application.Exit();
                }
            }
        }
        
        /// <summary>
        /// Inicializa el sistema de traducción según el idioma configurado en la base de datos.
        /// 
        /// Esta función es responsable de:
        /// 1. Obtener la configuración del sistema desde el servicio de configuración
        /// 2. Leer el idioma configurado (ej: "es-AR", "en-US")
        /// 3. Cargar el archivo de textos (Textos_Controles_UI.txt)
        /// 4. Inicializar el TranslationService con el idioma y textos
        /// 
        /// Si hay errores en cualquier paso, usa como fallback:
        /// - Idioma: "es-AR" (español argentino)
        /// - Archivo: Busca en la carpeta de ejecución de la aplicación
        /// 
        /// El archivo de textos debe estar en formato:
        /// Clave=Español|English|Portugués
        /// Ejemplo:
        /// Aceptar=Aceptar|Accept|Aceitar
        /// Cancelar=Cancelar|Cancel|Cancelar
        /// 
        /// Flujo de control:
        /// 1. Intenta obtener IConfiguracionService del contenedor
        /// 2. Si existe: carga configuración, obtiene idioma, carga archivo
        /// 3. Si falla: usa configuración por defecto
        /// 4. Manejo de excepciones: Bloquea errores no fatales e inicia con defaults
        /// </summary>
        private static void InicializarIdioma(IServiceProvider serviceProvider)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[Program] Iniciando InicializarIdioma()");
                
                var configuracionService = serviceProvider.GetService<IConfiguracionService>();
                
                if (configuracionService != null)
                {
                    System.Diagnostics.Debug.WriteLine("[Program] ConfiguracionService obtenido correctamente");
                    
                    var configuracion = configuracionService.ObtenerConfiguracion();
                    
                    System.Diagnostics.Debug.WriteLine($"[Program] Configuración obtenida: {(configuracion != null ? "SI" : "NO")}");
                    
                    string idioma = "es-AR"; // Idioma por defecto
                    
                    if (configuracion != null && !string.IsNullOrEmpty(configuracion.Idioma))
                    {
                        idioma = configuracion.Idioma;
                        System.Diagnostics.Debug.WriteLine($"[Program] Idioma de configuración: {idioma}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[Program] Usando idioma por defecto: {idioma}");
                    }
                    
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string translationFilePath = Path.Combine(basePath, "Textos_Controles_UI.txt");
                    
                    System.Diagnostics.Debug.WriteLine($"[Program] Ruta base: {basePath}");
                    System.Diagnostics.Debug.WriteLine($"[Program] Archivo de traducciones: {translationFilePath}");
                    System.Diagnostics.Debug.WriteLine($"[Program] ¿Archivo existe?: {File.Exists(translationFilePath)}");
                    
                    TranslationService.Initialize(idioma, translationFilePath);
                    
                    System.Diagnostics.Debug.WriteLine($"[Program] TranslationService inicializado con idioma: {idioma}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[Program] ConfiguracionService es NULL, usando idioma por defecto");
                    
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string translationFilePath = Path.Combine(basePath, "Textos_Controles_UI.txt");
                    TranslationService.Initialize("es-AR", translationFilePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Program] ERROR al inicializar idioma: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Program] StackTrace: {ex.StackTrace}");
                
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string translationFilePath = Path.Combine(basePath, "Textos_Controles_UI.txt");
                TranslationService.Initialize("es-AR", translationFilePath);
            }
        }
        
        /// <summary>
        /// Configura y registra todas las dependencias de inyección necesarias para la aplicación.
        /// 
        /// Este método es responsable de:
        /// 1. Obtener la cadena de conexión de App.config
        /// 2. Validar que la conexión esté disponible
        /// 3. Registrar todas las capas de negocio (BLL, DAL) mediante IoC.DependencyContainer
        /// 4. Registrar servicios heredados (Services)
        /// 5. Registrar todos los formularios como Transient (nueva instancia cada vez)
        /// 
        /// IMPORTANTE: Este método es llamado una sola vez durante la inicialización de la aplicación.
        /// Todas las dependencias se registran en el contenedor del que se extraerán durante
        /// la ejecución de la aplicación.
        /// 
        /// Orden de registración:
        /// 1. SqlServerHelper (Singleton): Helper para conexiones SQL
        /// 2. IoC.DependencyContainer.AddIoCDependencies: Registra BLL y DAL (punto de entrada de IoC)
        /// 3. Services.DependencyContainer.AddServicesDependencies: Registra servicios heredados
        /// 4. BLL.DependencyContainer.AddBLLDependencies: (Duplicado - ya registrado en paso 2)
        /// 5. Todos los formularios como Transient
        /// </summary>
        /// <param name="services">
        /// El contenedor de inyección de dependencias de Microsoft.Extensions.DependencyInjection
        /// donde se registrarán todas las dependencias de la aplicación.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Se lanza si no se encuentra la cadena de conexión "Huamani_SistemaPresupuestario"
        /// en la sección ConnectionStrings del archivo App.config.
        /// </exception>
        private static void InjectionServices(IServiceCollection services)
        {
            /// <summary>
            /// Obtiene la cadena de conexión de la configuración de la aplicación.
            /// 
            /// NOTA: La clave "Huamani_SistemaPresupuestario" debe estar definida en App.config:
            /// <configuration>
            ///   <connectionStrings>
            ///     <add name="Huamani_SistemaPresupuestario" 
            ///          connectionString="Data Source=SERVIDOR; ... " />
            ///   </connectionStrings>
            /// </configuration>
            /// 
            /// Si no se encuentra, se lanza una excepción con un mensaje claro para guiar
            /// al desarrollador a configurar la cadena de conexión correctamente.
            /// </summary>
            var csSetting = ConfigurationManager.ConnectionStrings["Huamani_SistemaPresupuestario"];
            if (csSetting == null)
                throw new InvalidOperationException("No se encontró la connection string 'SistemaPresupuestario' en App.config");
            
            /// <summary>
            /// Obtiene la sección AppSettings de App.config.
            /// Contiene configuraciones adicionales como parámetros de aplicación.
            /// Se pasa a Services.DependencyContainer para configurar servicios heredados.
            /// </summary>
            var app = ConfigurationManager.AppSettings;

            /// <summary>
            /// Registra todas las dependencias de la aplicación en encadenamiento fluido (fluent API).
            /// 
            /// ESTRUCTURA DE REGISTRACIÓN:
            /// 
            /// 1. SqlServerHelper (Singleton)
            ///    - Helper para operaciones con SQL Server
            ///    - Se crea una única instancia para toda la vida de la aplicación
            /// 
            /// 2. AddIoCDependencies(csSetting) - PUNTO DE ENTRADA PRINCIPAL DE IoC
            ///    Orquesta la registración de:
            ///    - BLL.DependencyContainer.AddBLLDependencies()
            ///      * AutoMapper con todos los perfiles de mapeo
            ///      * Todos los servicios de negocio (ClienteService, ProductoService, etc.)
            ///    - DAL.DependencyContainer.AddDALDependencies(csSetting)
            ///      * Entity Framework / Contexto de BD
            ///      * Todos los repositorios (ClienteRepository, ProductoRepository, etc.)
            /// 
            /// 3. AddServicesDependencies(csSetting, app) - Servicios heredados
            ///    Registra servicios de la capa Services (heredada, en transición)
            ///    - Servicios de negocio antiguos
            ///    - Servicios auxiliares como logging, cryptografía, etc.
            /// 
            /// 4. AddBLLDependencies() - NOTA: DUPLICADO
            ///    Ya fue registrado en el paso 2 (AddIoCDependencies)
            ///    Se recomienda eliminar esta línea en futuras refactorizaciones
            /// 
            /// 5. Todos los formularios como Transient
            ///    Cada vez que se solicita un formulario, se crea una nueva instancia
            ///    Esto permite que cada instancia del formulario tenga su propio estado
            ///    Formularios principales:
            ///    - frmLogin: Pantalla de autenticación
            ///    - frmMain: Ventana principal / MDI parent
            ///    - frmClientes, frmVendedores, frmProductos: Gestión de maestros
            ///    - frmPresupuesto: Gestión de cotizaciones
            ///    - frmFacturar: Emisión de facturas
            ///    - frmConfiguacionGeneral: Configuración del sistema
            /// </summary>
            services
            .AddSingleton<SqlServerHelper>()
            .AddIoCDependencies(csSetting)
                .AddServicesDependencies(csSetting, app)
                .AddBLLDependencies()
                .AddTransient<frmLogin>()
                .AddTransient<frmMain>()
                .AddTransient<frmUsuarios>()
                .AddTransient<frmAlta>()
                .AddTransient<frmClientes>()
                .AddTransient<frmClienteAlta>()
                .AddTransient<frmVendedores>()
                .AddTransient<frmVendedorAlta>()
                .AddTransient<frmProductos>()
                .AddTransient<frmListaPrecios>()
                .AddTransient<frmListaPrecioAlta>()
                .AddTransient<frmPresupuesto>()
                .AddTransient<frmConfiguacionGeneral>()
                .AddTransient<frmActualizarPadronArba>()
                .AddTransient<frmFacturar>();
        }
    }
}
