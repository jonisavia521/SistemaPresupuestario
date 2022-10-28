using Services.DAL.Implementations;
using Services.DAL.Implementations.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DAL.Factory
{
    internal class ServiceFactory
    {
        public ILoggerRepository loggerRepository;
        public ILanguageRepository languageRepository;
        public ServiceFactory(ILoggerRepository loggerRepository, ILanguageRepository languageRepository)
        {
            this.loggerRepository=loggerRepository;
            this.languageRepository =languageRepository;
        }     
    }
}
