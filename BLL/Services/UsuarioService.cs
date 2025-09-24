using AutoMapper;
using BLL.Contracts;
using BLL.DTOs;
using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using DAL.Implementation.Repository;
using DomainModel.Domain;
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

        public void Add(UsuarioDTO obj)
        {
            var usuarioDM = _mapper.Map<DomainModel.Domain.UsuarioDM>(obj);
            var usuario = _mapper.Map<Usuario>(usuarioDM);
            _unitOfWork.Usuarios.Add(usuario);
            _unitOfWork.SaveChanges();
        }

        public void Delete(UsuarioDTO obj)
        {
            // convertir DTO a dominio
            var usuarioDomain = _mapper.Map<DomainModel.Domain.UsuarioDM>(obj);
            // convertir dominio a Entity (EF)
            var usuarioEF = _mapper.Map<Usuario>(usuarioDomain);
            _unitOfWork.Usuarios.Delete(usuarioEF);
            _unitOfWork.SaveChanges();
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllAsync()
        {
            var usuariosEF = await _unitOfWork.Usuarios.GetAllAsync();
            var usuariosDomain = _mapper.Map<List<UsuarioDM>>(usuariosEF);
            var usuariosDTO = _mapper.Map<List<UsuarioDTO>>(usuariosDomain);
            return usuariosDTO;
        }

        public async Task<UsuarioDTO> GetByIdAsync(Guid id)
        {
            var usuarioEF = await _unitOfWork.Usuarios.GetByIdAsync(id);
            var usuarioDomain = _mapper.Map<UsuarioDM>(usuarioEF);
            var usuarioDTO = _mapper.Map<UsuarioDTO>(usuarioDomain);
            return usuarioDTO;
        }

        public void Update(UsuarioDTO obj)
        {
            var usuarioDomain = _mapper.Map<UsuarioDM>(obj);
            var usuarioEF = _mapper.Map<Usuario>(usuarioDomain);
            _unitOfWork.Usuarios.Update(usuarioEF);
            _unitOfWork.SaveChanges();
        }
    }
}
