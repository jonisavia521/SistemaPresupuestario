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
    /// Esta clase actúa como punto de entrada central para la configuración de todas las
    /// dependencias de la aplicación. Orquesta la registración de dependencias de todas las capas:
    /// - BLL (Lógica de Negocio)
    /// - DAL (Acceso a Datos)
    /// </summary>
    public static class DependencyContainer
    {
        
        public static IServiceCollection AddIoCDependencies(
            this IServiceCollection services,
            System.Configuration.ConnectionStringSettings csSetting)
        {
            services.AddBLLDependencies();

            services.AddDALDependencies(csSetting);

            return services;
        }
    }
}
