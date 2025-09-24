using Microsoft.Extensions.DependencyInjection;
using Services.BLL;
using Services.BLL.Contracts;
using Services.DAL.Contracts;
using Services.DAL.Factory;
using Services.DAL.Implementations;
using Services.DAL.Implementations.Adapter;
using Services.DAL.Implementations.Contracts;
using Services.DAL.Implementations.Joins;
using Services.DomainModel.Security.Composite;
using Services.Services;
using Services.Services.Contracts;
using Services.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddServicesDependencies(
           this IServiceCollection services
, System.Configuration.ConnectionStringSettings csSetting,
System.Collections.Specialized.NameValueCollection app)
        {
            services.AddSingleton(csSetting);
            services.AddSingleton(app);
            services.AddSingleton<ILogin,LoginServices>();
            services.AddSingleton<ILogger, LoggerService>();
            services.AddSingleton<ILoggerBLL, LoggerBLL>();
            services.AddSingleton<IExceptionBLL,ExceptionBLL>();

            services.AddSingleton<IGenericRepository<Familia>, FamiliaRepository>();
            services.AddSingleton<IGenericRepository<Usuario>, UsuarioRepository>();
            services.AddSingleton<IGenericRepository<Patente>, PatenteRepository>();            

            services.AddSingleton<IJoinRepository<Usuario>, UsuarioFamiliaRepository>();
            services.AddSingleton<IJoinRepository<Usuario>, UsuarioPatenteRepository>();

            services.AddSingleton<IJoinRepository<Familia>, FamiliaFamiliaRepository>();
            services.AddSingleton<IJoinRepository<Familia>, FamiliaPatenteRepository>();

            services.AddSingleton<IAdapter<Usuario>, UsuarioAdapter>();
            services.AddSingleton<IAdapter<Familia>, FamiliaAdapter>();
            services.AddSingleton<IAdapter<Patente>, PatenteAdapter>();

            services.AddSingleton<ILoggerRepository, LoggerRepository>();
            services.AddSingleton<ILanguageRepository, LanguageRepository>();

            services.AddSingleton<ServiceFactory>();
            services.AddSingleton<Usuario>();
            
            return services;
        }
    }
}
