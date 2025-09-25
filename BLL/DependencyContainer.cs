using AutoMapper;
using BLL.Contracts;
using BLL.Contracts.Seguridad;
using BLL.Mappers;
using BLL.Services;
using BLL.Services.Seguridad;
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
            
            // DECISIÓN: Agregar SeguridadProfile para mapeos de seguridad
            services.AddAutoMapper(typeof(UsuarioMapperProfile), typeof(SeguridadProfile));
            
            // Register the existing DTO-based service
            services.AddScoped<IUsuarioService, UsuarioService>();
            
            // NUEVOS SERVICIOS DE SEGURIDAD
            // DECISIÓN: Bypass de Service Layer - UI llamará directamente a BLL
            services.AddScoped<IUsuarioBusinessService, UsuarioBusinessService>();
            services.AddScoped<IPermisosBusinessService, PermisosBusinessService>();
            
            // Password hashing
            services.AddScoped<IPasswordHasher, SimpleSha256PasswordHasher>();
            
            // Logging básico
            services.AddSingleton<ILogger, SimpleFileLogger>();
            
            return services;
        }
    }
}
