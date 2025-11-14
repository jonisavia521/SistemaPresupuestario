using Services.BLL;
using Services.BLL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Extensions
{
    internal static class ExceptionExtension
    {
        public static void Handle(this Exception ex, object sender, IExceptionBLL exceptionBLL)
        {
            exceptionBLL.Handle(ex, sender);
        }
    }
}
