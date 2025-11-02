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
            // Registrar AutoMapper con los perfiles
            services.AddAutoMapper(typeof(ClienteMappingProfile), typeof(VendedorMappingProfile));
            
            // Registrar servicios
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IVendedorService, VendedorService>();
            
            return services;
        }
    }
}
