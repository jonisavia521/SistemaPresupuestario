using AutoMapper;
using DAL.Implementation.EntityFramework.Context;
using DAL.Repository;

using Microsoft.EntityFrameworkCore;
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
            //services.AddScoped<SistemaPresupuestarioContext>();
            services.AddAutoMapper();
            services.AddDbContext<SistemaPresupuestarioContext>(options => options.UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["ServicesConString"].ConnectionString));
            //services.AddSingleton<ILogger, LoggerService>();
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
