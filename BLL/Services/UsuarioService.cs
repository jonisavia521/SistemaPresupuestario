using BLL.Contracts;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    internal class UsuarioService : IGenericBusinessService<Usuario>
    {
        public void Add(Usuario obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Usuario> SelectAll()
        {
            throw new NotImplementedException();
        }

        public Usuario SelectOne(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(Usuario obj)
        {
            throw new NotImplementedException();
        }
    }
}
