
using DAL.Implementation.EntityFramework.Context;
using DAL.Implementation.Repository;
using DAL.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Services.Services;
using Services.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddDALDependencies(
           this IServiceCollection services
           )
        {
            services.AddTransient<SistemaPresupuestarioContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            return services;
        }
    }
}
