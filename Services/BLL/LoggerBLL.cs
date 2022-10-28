using Services.BLL.Contracts;
using Services.DAL.Factory;
using Services.DAL.Implementations.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BLL
{
    internal class LoggerBLL:ILoggerBLL
    {
        ServiceFactory serviceFactory;
        public LoggerBLL(ServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory;
        }
        public void WriteLog(string message, EventLevel level, string user)
        {
            serviceFactory.loggerRepository.WriteLog(message, level, user);

            //Definir acá las políticas de escritura..
            //switch(level)
            //{
            //    case EventLevel.Critical:
            //        //Política de manejo de errores...

            //        break;

            //    case EventLevel.Warning:
            //        break;
            //}

        }
    }
}
