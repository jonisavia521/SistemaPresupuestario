using AutoMapper;
using BLL.Contracts;
using BLL.DTOs;
using DAL.Contracts;
using DAL.Implementation.EntityFramework.Context;
using DAL.Implementation.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsuarioService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(UsuarioDTO obj)
        {
            var usuario = _mapper.Map<Usuario>(obj);
            if (usuario.IdUsuario == Guid.Empty)
            {
                usuario.IdUsuario = Guid.NewGuid();
            }            
            await _unitOfWork.Usuarios.AddAsync(usuario);
            _unitOfWork.SaveChanges();
        }

        public void Delete(UsuarioDTO obj)
        {
            // convertir DTO a dominio
            var usuarioDomain = _mapper.Map<DomainModel.Domain.Usuario>(obj);
            // convertir dominio a Entity (EF)
            var usuarioEF = _mapper.Map<Usuario>(usuarioDomain);
            _unitOfWork.Usuarios.Delete(usuarioEF);
            _unitOfWork.SaveChanges();
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllAsync()
        {
            var usuariosEF = await _unitOfWork.Usuarios.GetAllAsync();
            var usuariosDomain = _mapper.Map<List<DomainModel.Domain.Usuario>>(usuariosEF);
            var usuariosDTO = _mapper.Map<List<UsuarioDTO>>(usuariosDomain);
            return usuariosDTO;
        }

        public async Task<UsuarioDTO> GetByIdAsync(Guid id)
        {
            var usuarioEF = await _unitOfWork.Usuarios.GetByIdAsync(id);
            var usuarioDomain = _mapper.Map<DomainModel.Domain.Usuario>(usuarioEF);
            var usuarioDTO = _mapper.Map<UsuarioDTO>(usuarioDomain);
            return usuarioDTO;
        }

        public void Update(UsuarioDTO obj)
        {
            var usuarioDomain = _mapper.Map<DomainModel.Domain.Usuario>(obj);
            var usuarioEF = _mapper.Map<Usuario>(usuarioDomain);
            _unitOfWork.Usuarios.Update(usuarioEF);
            _unitOfWork.SaveChanges();
        }
    }
}
