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
    public static class DependencyContainer
    {
        // Este es el ÚNICO método que llamará la UI
        public static IServiceCollection AddIoCDependencies(
            this IServiceCollection services,
            System.Configuration.ConnectionStringSettings csSetting)
        {
            // 1. Llama al registro de la capa BLL
            services.AddBLLDependencies();

            // 2. Llama al registro de la capa DAL
            services.AddDALDependencies(csSetting);

            // (Si tuvieras más capas, como Caching, Email, etc., las llamas aquí)

            return services;
        }
    }
}
