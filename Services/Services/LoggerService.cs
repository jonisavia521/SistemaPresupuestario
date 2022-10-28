using Services.BLL.Contracts;
using Services.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    internal class LoggerService:ILogger
    {
        ILoggerBLL _logBLL;
        public LoggerService(ILoggerBLL logBLL)
        {
            _logBLL = logBLL;
        }
        public void WriteLog(string message, EventLevel level, string user)
        {
            _logBLL.WriteLog(message, level, user);
        }
    }
}
