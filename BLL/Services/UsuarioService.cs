using AutoMapper;
using BLL.Contracts;
using BLL.DTOs;
using DAL.Implementation.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly SistemaPresupuestarioContext _context;
        private readonly IMapper _mapper;

        public UsuarioService(SistemaPresupuestarioContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void Add(UsuarioDTO obj)
        {
            var usuario = _mapper.Map<Usuario>(obj);
            if (usuario.IdUsuario == Guid.Empty)
            {
                usuario.IdUsuario = Guid.NewGuid();
            }
            
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var usuario = _context.Usuario.Find(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
                _context.SaveChanges();
            }
        }

        public IEnumerable<UsuarioDTO> GetAll()
        {
            var usuarios = _context.Usuario.ToList();
            return _mapper.Map<List<UsuarioDTO>>(usuarios);
        }

        public UsuarioDTO GetById(Guid id)
        {
            var usuario = _context.Usuario.Find(id);
            return usuario != null ? _mapper.Map<UsuarioDTO>(usuario) : null;
        }

        public void Update(UsuarioDTO obj)
        {
            var usuario = _mapper.Map<Usuario>(obj);
            _context.Usuario.Update(usuario);
            _context.SaveChanges();
        }
    }
}
