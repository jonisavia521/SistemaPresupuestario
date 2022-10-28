
using BLL;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario
{
    static class Program
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
            services                
                .AddServicesDependencies()
                .AddBLLDependencies()
                .AddScoped<frmLogin>()
                .AddScoped<frmMain>();
            
        }

    }
}
