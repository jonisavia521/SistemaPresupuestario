using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Contracts
{
    public interface IRemoveController<T>
    {
        void Remove(Guid id);
        void Remove(int id);
    }
}
