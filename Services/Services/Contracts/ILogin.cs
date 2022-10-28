using Services.DomainModel.Security.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Contracts
{
    public interface ILogin
    {
        Usuario user { get; }
        bool Login(string usuario, string clave);
    }
}
