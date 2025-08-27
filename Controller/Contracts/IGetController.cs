using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Contracts
{
    public interface IGetController<T>
    {
        IEnumerable<T> GetAll();
        T GetById(Guid id);
        T GetById(int id);
    }
}
