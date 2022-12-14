using Services.BLL.Contracts;
using Services.Services;
using Services.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BLL
{
    internal class ExceptionBLL: IExceptionBLL
    {
        private static string dalAssembly = ConfigurationManager.AppSettings["DALAssembly"];
        private static string bllAssembly = ConfigurationManager.AppSettings["BLLAssembly"];
        private ILogger _log;
        public ExceptionBLL(ILogger log)
        {
            _log = log;
        }
        public void Handle(Exception ex, object sender)
        {
            //Aplicar nuestras políticas de Exceptions
            Console.WriteLine(ex.Message);

            string assemblyName = sender.GetType().Module.Name;

            if (assemblyName == dalAssembly)
            {
                //Aplicamos la política de exception de la DAL
                DALPolicy(ex);
            }
            else if (assemblyName == bllAssembly)
            {
                BLLPolicy(ex);
            }
        }

        private void DALPolicy(Exception ex)
        {
            //1) Registrar
            _log.WriteLog($"Message; {ex.Message}, StackTrace: {ex.StackTrace}", EventLevel.Error, String.Empty);
            //2) Propagar
            throw new Exception(String.Empty, ex);
        }

        private void BLLPolicy(Exception ex)
        {
            //Tengo que saber si la exception viene de BLL puramente o de DAL
            if (ex.InnerException != null)
            {
                //Estoy ante una exception en BLL pero que fue originada en DAL
                throw new Exception("Error accediendo a los datos...", ex);
            }
            else
            {
                //Es una exception propia de BLL
                _log.WriteLog($"Message; {ex.Message}, StackTrace: {ex.StackTrace}", EventLevel.Error, String.Empty);
                //2) Propagar
                throw ex;
            }
        }
    }
}
