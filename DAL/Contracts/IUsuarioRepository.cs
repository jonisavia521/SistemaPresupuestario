using DAL.Implementation.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface IUsuarioRepository
    {
        void Add(Usuario usuario);
        void Update(Usuario usuario);
        void Delete(Guid id);
        IEnumerable<Usuario> GetAll();
        Usuario GetById(Guid id);
    }
}