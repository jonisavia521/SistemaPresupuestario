using Services.DomainModel.Security.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Contracts
{
    public interface IPatenteService
    {
        IEnumerable<Patente> GetAll();
        IEnumerable<Patente> GetByUsuario(Guid idUsuario);
    }
}
