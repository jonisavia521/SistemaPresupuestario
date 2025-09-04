using DAL.Contracts;
using DAL.Implementation.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DAL.Implementation.Repository
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario> GetByEmailAsync(string email);
    }

    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(SistemaPresupuestarioContext context) : base(context) { }

        public async Task<Usuario> GetByEmailAsync(string usuario)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Usuario1 == usuario);
        }
    }
}
