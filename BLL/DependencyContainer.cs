using AutoMapper;
using BLL.Contracts;
using BLL.Mappers;
using BLL.Services;
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
        public static IServiceCollection AddBLLDependencies(this IServiceCollection services)
        {
            // Registrar AutoMapper con el perfil de Cliente
            services.AddAutoMapper(typeof(ClienteMappingProfile));
            
            // Registrar el servicio de Cliente
            services.AddScoped<IClienteService, ClienteService>();
            
            return services;
        }
    }
}
