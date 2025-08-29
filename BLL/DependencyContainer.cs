using AutoMapper;
using BLL.Contracts;
using BLL.Mappers;
using BLL.Services;
using DAL;
using DomainModel.Domain;
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
            services.AddDALDependencies();
            services.AddAutoMapper(typeof(UsuarioMapperProfile));
            
            // Register the new DTO-based service
            services.AddScoped<IUsuarioService, UsuarioService>();
            
            return services;
        }
    }
}
