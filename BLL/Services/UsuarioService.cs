using AutoMapper;
using BLL.Contracts;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Security;
using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UsuarioService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        #region Métodos básicos (compatibilidad)
        public void Add(UsuarioDTO obj)
        {
            var usuarioDM = _mapper.Map<DomainModel.Domain.UsuarioDM>(obj);
            var usuario = _mapper.Map<Usuario>(usuarioDM);
            _unitOfWork.Usuarios.Add(usuario);
            _unitOfWork.SaveChanges();
        }

        public void Delete(UsuarioDTO obj)
        {
            var usuarioDomain = _mapper.Map<DomainModel.Domain.UsuarioDM>(obj);
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
        #endregion

        #region Métodos extendidos para ABM completo
        public async Task<UserEditDto> GetUserForEditAsync(Guid id)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdWithRelationsAsync(id);
            if (usuario == null)
                throw new ArgumentException($"Usuario con ID {id} no encontrado");

            return _mapper.Map<UserEditDto>(usuario);
        }

        public async Task<bool> ExistsUserNameAsync(string userName, Guid? excludeId = null)
        {
            return await _unitOfWork.Usuarios.ExistsUserNameAsync(userName, excludeId);
        }

        public async Task<Guid> CreateUserWithRelationsAsync(UserEditDto userDto)
        {
            // Validaciones de negocio
            await ValidateUserBusinessRules(userDto);

            _unitOfWork.BeginTransaction();
            try
            {
                // Crear usuario
                var usuario = new Usuario
                {
                    IdUsuario = Guid.NewGuid(),
                    Nombre = userDto.Nombre,
                    Usuario1 = userDto.Usuario,
                    Clave = _passwordHasher.HashPassword(userDto.Clave)
                };

                _unitOfWork.Usuarios.Add(usuario);
                
                // Asignar relaciones
                _unitOfWork.Usuarios.UpdateUserWithRelations(usuario, 
                    userDto.FamiliasAsignadas, 
                    userDto.PatentesAsignadas);

                _unitOfWork.Commit();
                return usuario.IdUsuario;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task UpdateUserWithRelationsAsync(UserEditDto userDto)
        {
            // Validaciones de negocio
            await ValidateUserBusinessRules(userDto);

            var existingUser = await _unitOfWork.Usuarios.GetByIdAsync(userDto.Id);
            if (existingUser == null)
                throw new ArgumentException($"Usuario con ID {userDto.Id} no encontrado");

            // Verificar concurrencia
            if (!userDto.Timestamp.SequenceEqual(existingUser.timestamp))
                throw new ConcurrencyException("Usuario", userDto.Id, 
                    "El usuario ha sido modificado por otro usuario. Recargue los datos e intente nuevamente.");

            _unitOfWork.BeginTransaction();
            try
            {
                // Actualizar datos básicos
                existingUser.Nombre = userDto.Nombre;
                existingUser.Usuario1 = userDto.Usuario;
                
                // Actualizar clave si se requiere
                if (userDto.CambiarClave && !string.IsNullOrEmpty(userDto.Clave))
                {
                    existingUser.Clave = _passwordHasher.HashPassword(userDto.Clave);
                }

                // Actualizar relaciones
                _unitOfWork.Usuarios.UpdateUserWithRelations(existingUser, 
                    userDto.FamiliasAsignadas, 
                    userDto.PatentesAsignadas);

                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task DeleteUserAsync(Guid id, byte[] timestamp)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
            if (usuario == null)
                throw new ArgumentException($"Usuario con ID {id} no encontrado");

            // Verificar concurrencia
            if (!timestamp.SequenceEqual(usuario.timestamp))
                throw new ConcurrencyException("Usuario", id, 
                    "El usuario ha sido modificado por otro usuario. Recargue los datos e intente nuevamente.");

            _unitOfWork.BeginTransaction();
            try
            {
                _unitOfWork.Usuarios.Delete(usuario);
                _unitOfWork.Commit();
            }
            catch (DbUpdateException ex)
            {
                _unitOfWork.Rollback();
                throw new DomainValidationException(
                    "No se puede eliminar el usuario porque tiene dependencias en el sistema.");
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<PermisoEfectivoDto> GetEffectivePermissionsAsync(Guid userId)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdWithRelationsAsync(userId);
            if (usuario == null)
                throw new ArgumentException($"Usuario con ID {userId} no encontrado");

            var permiso = new PermisoEfectivoDto
            {
                IdUsuario = userId,
                PatentesDirectas = _mapper.Map<List<PatenteDto>>(
                    usuario.Usuario_Patente.Select(up => up.Patente)),
                FamiliasAsignadas = _mapper.Map<List<FamiliaDto>>(
                    usuario.Usuario_Familia.Select(uf => uf.Familia))
            };

            // Calcular patentes heredadas
            var patentesHeredadas = new List<PatenteHeredadaDto>();
            foreach (var familiaAsignada in permiso.FamiliasAsignadas)
            {
                var patentesDeEstaFamilia = await GetPatentesHeredadasDeFamilia(familiaAsignada.IdFamilia, familiaAsignada.Nombre);
                patentesHeredadas.AddRange(patentesDeEstaFamilia);
            }

            permiso.PatentesHeredadas = patentesHeredadas.DistinctBy(p => p.Patente.IdPatente).ToList();

            // Calcular total único
            var todasLasPatentes = permiso.PatentesDirectas.Select(p => p.IdPatente)
                .Union(permiso.PatentesHeredadas.Select(p => p.Patente.IdPatente))
                .Distinct();
            permiso.TotalPermisosUnicos = todasLasPatentes.Count();

            return permiso;
        }

        public async Task<IEnumerable<UserEditDto>> GetPagedUsersAsync(string filter, int page, int pageSize, out int total)
        {
            var usuarios = await _unitOfWork.Usuarios.GetPagedAsync(filter, page, pageSize, out total);
            return _mapper.Map<List<UserEditDto>>(usuarios);
        }

        public async Task<IEnumerable<FamiliaDto>> GetAllFamiliasAsync()
        {
            var familias = await _unitOfWork.Familias.GetAllWithRelationsAsync();
            return _mapper.Map<List<FamiliaDto>>(familias);
        }

        public async Task<IEnumerable<PatenteDto>> GetAllPatentesAsync()
        {
            var patentes = await _unitOfWork.Patentes.GetAllWithRelationsAsync();
            return _mapper.Map<List<PatenteDto>>(patentes);
        }

        public async Task<List<FamiliaDto>> GetFamiliasJerarquicasAsync()
        {
            var familias = await _unitOfWork.Familias.GetFamiliasJerarquicasAsync();
            return _mapper.Map<List<FamiliaDto>>(familias);
        }
        #endregion

        #region Métodos privados de apoyo
        private async Task ValidateUserBusinessRules(UserEditDto userDto)
        {
            // Validar nombre único
            if (await ExistsUserNameAsync(userDto.Usuario, userDto.EsNuevo ? null : userDto.Id))
                throw new DomainValidationException(nameof(userDto.Usuario), userDto.Usuario, 
                    "Ya existe un usuario con este nombre de usuario");

            // Validar que tenga al menos un permiso
            if (!userDto.FamiliasAsignadas.Any() && !userDto.PatentesAsignadas.Any())
                throw new DomainValidationException("Permisos", null, 
                    "El usuario debe tener al menos una familia o patente asignada");

            // Validar clave para usuarios nuevos
            if (userDto.EsNuevo && string.IsNullOrWhiteSpace(userDto.Clave))
                throw new DomainValidationException(nameof(userDto.Clave), userDto.Clave, 
                    "La clave es obligatoria para usuarios nuevos");

            // Validar confirmación de clave
            if (!string.IsNullOrEmpty(userDto.Clave) && userDto.Clave != userDto.ConfirmarClave)
                throw new DomainValidationException(nameof(userDto.ConfirmarClave), userDto.ConfirmarClave, 
                    "La confirmación de clave no coincide con la clave");
        }

        private async Task<List<PatenteHeredadaDto>> GetPatentesHeredadasDeFamilia(Guid familiaId, string nombreFamilia)
        {
            var patentesIds = await _unitOfWork.Familias.GetPatentesFromFamiliaAsync(familiaId);
            var patentes = new List<PatenteHeredadaDto>();

            foreach (var patenteId in patentesIds)
            {
                var patente = await _unitOfWork.Patentes.GetByIdAsync(patenteId);
                if (patente != null)
                {
                    patentes.Add(new PatenteHeredadaDto
                    {
                        Patente = _mapper.Map<PatenteDto>(patente),
                        OrigenFamilia = nombreFamilia,
                        IdFamiliaOrigen = familiaId
                    });
                }
            }

            return patentes;
        }
        #endregion
    }
}
