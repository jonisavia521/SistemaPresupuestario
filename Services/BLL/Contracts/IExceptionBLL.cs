using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BLL.Contracts
{
    internal interface IExceptionBLL
    {
        void Handle(Exception ex, object sender);

    }
}
