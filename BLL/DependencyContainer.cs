using AutoMapper;
using BLL.Contracts;
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
            
            return services;
        }
    }
}
