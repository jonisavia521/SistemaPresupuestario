using DAL.Contracts;
using DAL.Implementation.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SistemaPresupuestarioContext _context;

        public UsuarioRepository(SistemaPresupuestarioContext context)
        {
            _context = context;
        }

        public void Add(Usuario usuario)
        {
            _context.Usuario.Add(usuario);
        }

        public void Delete(Guid id)
        {
            var usuario = _context.Usuario.Find(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
            }
        }

        public IEnumerable<Usuario> GetAll()
        {
            return _context.Usuario.ToList();
        }

        public Usuario GetById(Guid id)
        {
            return _context.Usuario.Find(id);
        }

        public void Update(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
        }
    }
}