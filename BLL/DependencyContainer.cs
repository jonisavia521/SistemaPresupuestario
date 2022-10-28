using Microsoft.Extensions.DependencyInjection;
using Services.Services;
using Services.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddBLLDependencies(
           this IServiceCollection services
           )
        {
            
            //services.AddSingleton<ILogger, LoggerService>();
            return services;
        }
    }
}
