using BLL;
using Controller.Contracts;
using Controller.Controllers;
using Controller.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
namespace Controller
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddControllerDependencies(
           this IServiceCollection services
           )
        {
            services.AddAutoMapper();
            services.AddScoped<ICRUDController<UsuarioView>,UsuarioController>();
            services.AddBLLDependencies();
            return services;
        }
    }
}
