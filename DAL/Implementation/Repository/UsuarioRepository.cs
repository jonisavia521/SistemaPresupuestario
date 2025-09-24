using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementation.Repository
{

    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Usuario GetByEmailAsync(string email);
    }

    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(SistemaPresupuestario context) : base(context) { }

        public Usuario GetByEmailAsync(string usuario)
        {
            return _dbSet.FirstOrDefault(u => u.Usuario1 == usuario);
        }
    }
}
