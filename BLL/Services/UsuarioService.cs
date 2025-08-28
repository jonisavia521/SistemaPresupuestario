using AutoMapper;
using BLL.Contracts;
using BLL.DTOs;
using DAL.Contracts;
using DAL.Implementation.EntityFramework.Context;
using DAL.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(UsuarioDTO obj)
        {
            var usuario = _mapper.Map<Usuario>(obj);
            if (usuario.IdUsuario == Guid.Empty)
            {
                usuario.IdUsuario = Guid.NewGuid();
            }
            
            _usuarioRepository.Add(usuario);
            _unitOfWork.SaveChanges();
        }

        public void Delete(Guid id)
        {
            _usuarioRepository.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<UsuarioDTO> GetAll()
        {
            var usuarios = _usuarioRepository.GetAll();
            return _mapper.Map<List<UsuarioDTO>>(usuarios);
        }

        public UsuarioDTO GetById(Guid id)
        {
            var usuario = _usuarioRepository.GetById(id);
            return usuario != null ? _mapper.Map<UsuarioDTO>(usuario) : null;
        }

        public void Update(UsuarioDTO obj)
        {
            var usuario = _mapper.Map<Usuario>(obj);
            _usuarioRepository.Update(usuario);
            _unitOfWork.SaveChanges();
        }
    }
}
