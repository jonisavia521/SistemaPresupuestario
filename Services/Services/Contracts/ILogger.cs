using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Contracts
{
    public interface ILogger
    {
        void WriteLog(string message, EventLevel level, string user);
    }
}
