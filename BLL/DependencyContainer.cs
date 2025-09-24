using AutoMapper;
using BLL.Contracts;
using BLL.Mappers;
using BLL.Security;
using BLL.Services;
using DAL;
using DomainModel.Domain;
using Microsoft.Extensions.DependencyInjection;
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
, System.Configuration.ConnectionStringSettings csSetting)
        {
            services.AddDALDependencies(csSetting);
            
            // AutoMapper profiles
            services.AddAutoMapper(typeof(UsuarioMapperProfile), typeof(SeguridadProfile));
            
            // Security services
            services.AddScoped<IPasswordHasher, SimplePasswordHasher>();
            
            // Register the new DTO-based service
            services.AddScoped<IUsuarioService, UsuarioService>();
            
            return services;
        }
    }
}
