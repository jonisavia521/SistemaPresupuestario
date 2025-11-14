using BLL;
using BLL.Contracts;
using IoC;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.DAL.Tools;
using Services.Services;
using SistemaPresupuestario.Configuracion;
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
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            var services = new ServiceCollection();
            InjectionServices(services);
            
            using (var serviceProvider = services.BuildServiceProvider())
            {
                InicializarIdioma(serviceProvider);
                
                var formLogin = serviceProvider.GetRequiredService<frmLogin>();
                if (formLogin.ShowDialog() == DialogResult.OK)
                {
                    var form = serviceProvider.GetRequiredService<frmMain>();
                    Application.Run(form);
                }
                else
                {
                    Application.Exit();
                }
            }
        }
        
        /// <summary>
        /// Inicializa el sistema de traducción según el idioma configurado en la base de datos
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
        
        private static void InjectionServices(IServiceCollection services)
        {
            var csSetting = ConfigurationManager.ConnectionStrings["Huamani_SistemaPresupuestario"];
            if (csSetting == null)
                throw new InvalidOperationException("No se encontró la connection string 'SistemaPresupuestario' en App.config");
            var app = ConfigurationManager.AppSettings;

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
