using BLL;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.DAL.Tools;
using SistemaPresupuestario.Maestros;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static void InjectionServices(IServiceCollection services)
        {
            var csSetting = ConfigurationManager.ConnectionStrings["SistemaPresupuestario"];
            if (csSetting == null)
                throw new InvalidOperationException("No se encontró la connection string 'SistemaPresupuestario' en App.config");
            var app = ConfigurationManager.AppSettings;

            services
            .AddSingleton<SqlServerHelper>()
                .AddServicesDependencies(csSetting, app)
                .AddBLLDependencies(csSetting)
                .AddTransient<frmLogin>()
                .AddTransient<frmMain>()
                .AddTransient<frmUsuarios>()
                .AddTransient<frmAlta>();


        }

    }
}
