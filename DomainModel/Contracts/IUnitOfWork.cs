using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Contracts { 
    //esto es para operar con la base de manera atomica.
    public interface IUnitOfWork
    {
        Task<int> SaveChanges();
    }
}
