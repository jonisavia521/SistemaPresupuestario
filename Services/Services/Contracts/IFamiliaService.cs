using Services.DomainModel.Security.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Contracts
{
    public interface IFamiliaService
    {
        IEnumerable<Familia> GetAll();
        IEnumerable<Familia> GetByUsuario(Guid idUsuario);
    }
}
