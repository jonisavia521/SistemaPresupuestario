using AutoMapper;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Infrastructure;
using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Seguridad
{
    /// <summary>
    /// Implementación del servicio de lógica de negocio para usuarios
    /// DECISIÓN: Usar async/await para preparar escalabilidad futura
    /// </summary>
    public class UsuarioBusinessService : IUsuarioBusinessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPermisosBusinessService _permisosService;
        private readonly ILogger _logger;

        public UsuarioBusinessService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPasswordHasher passwordHasher,
            IPermisosBusinessService permisosService,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _permisosService = permisosService;
            _logger = logger;
        }

        public async Task<(List<UserDto> usuarios, int total)> GetUsuariosPaginatedAsync(
            string filtroUsuario, string filtroNombre, int page, int pageSize)
        {
            try
            {
                var usuarios = _unitOfWork.Usuarios.GetPaged(filtroUsuario, filtroNombre, page, pageSize, out int total);
                var userDtos = new List<UserDto>();

                foreach (var usuario in usuarios)
                {
                    var userDto = _mapper.Map<UserDto>(usuario);
                    
                    // Calcular conteos de permisos
                    var conteos = await _permisosService.CountPermisosEfectivosAsync(usuario.IdUsuario);
                    userDto.CantPermisosDirectos = conteos.directos;
                    userDto.CantPermisosEfectivos = conteos.efectivos;
                    userDto.TimestampBase64 = Convert.ToBase64String(usuario.timestamp);
                    
                    userDtos.Add(userDto);
                }

                return (userDtos, total);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener usuarios paginados: {ex.Message}", ex, nameof(GetUsuariosPaginatedAsync));
                throw;
            }
        }

        public async Task<UserEditDto> GetUsuarioForEditAsync(Guid id)
        {
            try
            {
                var usuario = _unitOfWork.Usuarios.GetByIdWithRelations(id);
                if (usuario == null)
                    throw NotFoundException.ForUsuario(id);

                var userEditDto = _mapper.Map<UserEditDto>(usuario);
                userEditDto.TimestampBase64 = Convert.ToBase64String(usuario.timestamp);

                // Cargar familias asignadas
                userEditDto.FamiliasAsignadasIds = usuario.Usuario_Familia
                    .Select(uf => uf.IdFamilia)
                    .ToList();

                // Cargar patentes asignadas
                userEditDto.PatentesAsignadasIds = usuario.Usuario_Patente
                    .Select(up => up.IdPatente)
                    .ToList();

                return userEditDto;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener usuario para edición: {ex.Message}", ex, nameof(GetUsuarioForEditAsync));
                throw;
            }
        }

        public async Task<Guid> CreateUsuarioAsync(UserEditDto userEditDto)
        {
            try
            {
                // Validar datos básicos
                await ValidateUserDataAsync(userEditDto, isNew: true);

                // Validar que tenga permisos efectivos
                await _permisosService.ValidatePermissionAssignmentAsync(
                    Guid.Empty, userEditDto.FamiliasAsignadasIds, userEditDto.PatentesAsignadasIds);

                _unitOfWork.BeginTransaction();

                try
                {
                    // Crear entidad usuario
                    var usuarioEntity = new Usuario
                    {
                        IdUsuario = Guid.NewGuid(),
                        Nombre = userEditDto.Nombre?.Trim(),
                        Usuario1 = userEditDto.Usuario?.Trim(),
                        Clave = _passwordHasher.HashPassword(userEditDto.NuevaClaveOpcional)
                    };

                    // Agregar con relaciones
                    _unitOfWork.Usuarios.AddUserWithRelations(
                        usuarioEntity, 
                        userEditDto.FamiliasAsignadasIds, 
                        userEditDto.PatentesAsignadasIds);

                    _unitOfWork.Commit();

                    _logger.Info($"Usuario creado exitosamente: {userEditDto.Usuario}", nameof(CreateUsuarioAsync));
                    return usuarioEntity.IdUsuario;
                }
                catch
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al crear usuario: {ex.Message}", ex, nameof(CreateUsuarioAsync));
                throw;
            }
        }

        public async Task UpdateUsuarioAsync(UserEditDto userEditDto)
        {
            try
            {
                // Validar datos básicos
                await ValidateUserDataAsync(userEditDto, isNew: false);

                // Validar que tenga permisos efectivos
                await _permisosService.ValidatePermissionAssignmentAsync(
                    userEditDto.Id, userEditDto.FamiliasAsignadasIds, userEditDto.PatentesAsignadasIds);

                _unitOfWork.BeginTransaction();

                try
                {
                    var usuarioEntity = new Usuario
                    {
                        IdUsuario = userEditDto.Id,
                        Nombre = userEditDto.Nombre?.Trim(),
                        Usuario1 = userEditDto.Usuario?.Trim()
                    };

                    // Solo actualizar clave si se proporcionó una nueva
                    if (!string.IsNullOrEmpty(userEditDto.NuevaClaveOpcional))
                    {
                        usuarioEntity.Clave = _passwordHasher.HashPassword(userEditDto.NuevaClaveOpcional);
                    }

                    var timestamp = Convert.FromBase64String(userEditDto.TimestampBase64);

                    _unitOfWork.Usuarios.UpdateUserWithRelations(
                        usuarioEntity,
                        userEditDto.FamiliasAsignadasIds,
                        userEditDto.PatentesAsignadasIds,
                        timestamp);

                    _unitOfWork.Commit();

                    _logger.Info($"Usuario actualizado exitosamente: {userEditDto.Usuario}", nameof(UpdateUsuarioAsync));
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("modificado por otro usuario"))
                {
                    _unitOfWork.Rollback();
                    throw new ConcurrencyException(
                        "El usuario ha sido modificado por otro usuario",
                        userEditDto.Id,
                        "Usuario");
                }
                catch
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al actualizar usuario: {ex.Message}", ex, nameof(UpdateUsuarioAsync));
                throw;
            }
        }

        public async Task DeleteUsuarioAsync(Guid id, string timestampBase64)
        {
            try
            {
                var timestamp = Convert.FromBase64String(timestampBase64);

                _unitOfWork.BeginTransaction();

                try
                {
                    _unitOfWork.Usuarios.Remove(id, timestamp);
                    _unitOfWork.Commit();

                    _logger.Info($"Usuario eliminado exitosamente: {id}", nameof(DeleteUsuarioAsync));
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("modificado por otro usuario"))
                {
                    _unitOfWork.Rollback();
                    throw new ConcurrencyException(
                        "El usuario ha sido modificado por otro usuario",
                        id,
                        "Usuario");
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("no encontrado"))
                {
                    _unitOfWork.Rollback();
                    throw NotFoundException.ForUsuario(id);
                }
                catch
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al eliminar usuario: {ex.Message}", ex, nameof(DeleteUsuarioAsync));
                throw;
            }
        }

        public async Task<bool> IsUsuarioUniqueAsync(string usuario, Guid? excludeId = null)
        {
            try
            {
                return !_unitOfWork.Usuarios.ExistsUserName(usuario, excludeId);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al verificar unicidad de usuario: {ex.Message}", ex, nameof(IsUsuarioUniqueAsync));
                throw;
            }
        }

        public bool ValidatePasswordStrength(string password)
        {
            return _passwordHasher.ValidatePasswordStrength(password);
        }

        public string GetPasswordRequirements()
        {
            return _passwordHasher.GetPasswordRequirements();
        }

        public async Task<UserDto> AuthenticateAsync(string usuario, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
                    return null;

                var usuarioEntity = _unitOfWork.Usuarios.GetByEmailAsync(usuario);
                if (usuarioEntity == null)
                    return null;

                // Verificar contraseña
                if (!_passwordHasher.VerifyPassword(password, usuarioEntity.Clave))
                    return null;

                // DECISIÓN: Migrar contraseñas legacy durante login exitoso
                if (_passwordHasher is SimpleSha256PasswordHasher hasher && hasher.IsLegacyFormat(usuarioEntity.Clave))
                {
                    usuarioEntity.Clave = hasher.MigrateLegacyPassword(password);
                    _unitOfWork.SaveChanges();
                    _logger.Info($"Contraseña migrada para usuario: {usuario}", nameof(AuthenticateAsync));
                }

                var userDto = _mapper.Map<UserDto>(usuarioEntity);
                var conteos = await _permisosService.CountPermisosEfectivosAsync(usuarioEntity.IdUsuario);
                userDto.CantPermisosDirectos = conteos.directos;
                userDto.CantPermisosEfectivos = conteos.efectivos;

                return userDto;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error en autenticación: {ex.Message}", ex, nameof(AuthenticateAsync));
                return null; // No exponer detalles del error en autenticación
            }
        }

        public async Task ChangePasswordAsync(Guid usuarioId, string currentPassword, string newPassword)
        {
            try
            {
                var usuario = _unitOfWork.Usuarios.GetById(usuarioId);
                if (usuario == null)
                    throw NotFoundException.ForUsuario(usuarioId);

                // Verificar contraseña actual
                if (!_passwordHasher.VerifyPassword(currentPassword, usuario.Clave))
                    throw new DomainValidationException("La contraseña actual es incorreta", "CurrentPassword");

                // Validar nueva contraseña
                if (!_passwordHasher.ValidatePasswordStrength(newPassword))
                    throw new DomainValidationException(_passwordHasher.GetPasswordRequirements(), "NewPassword");

                // Actualizar contraseña
                usuario.Clave = _passwordHasher.HashPassword(newPassword);
                _unitOfWork.SaveChanges();

                _logger.Info($"Contraseña cambiada para usuario: {usuarioId}", nameof(ChangePasswordAsync));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al cambiar contraseña: {ex.Message}", ex, nameof(ChangePasswordAsync));
                throw;
            }
        }

        private async Task ValidateUserDataAsync(UserEditDto userEditDto, bool isNew)
        {
            var errors = new List<string>();

            // Validar nombre
            if (string.IsNullOrWhiteSpace(userEditDto.Nombre))
                errors.Add("El nombre es obligatorio");
            else if (userEditDto.Nombre.Trim().Length < 2)
                errors.Add("El nombre debe tener al menos 2 caracteres");

            // Validar usuario
            if (string.IsNullOrWhiteSpace(userEditDto.Usuario))
                errors.Add("El nombre de usuario es obligatorio");
            else if (userEditDto.Usuario.Trim().Length < 3)
                errors.Add("El usuario debe tener al menos 3 caracteres");
            else if (!await IsUsuarioUniqueAsync(userEditDto.Usuario, isNew ? null : userEditDto.Id))
                errors.Add("El nombre de usuario ya existe");

            // Validar contraseña (obligatoria en alta, opcional en edición)
            if (isNew || !string.IsNullOrEmpty(userEditDto.NuevaClaveOpcional))
            {
                if (string.IsNullOrEmpty(userEditDto.NuevaClaveOpcional))
                    errors.Add("La contraseña es obligatoria");
                else if (!ValidatePasswordStrength(userEditDto.NuevaClaveOpcional))
                    errors.Add(GetPasswordRequirements());
                else if (!userEditDto.ValidarClaves())
                    errors.Add("Las contraseñas no coinciden");
            }

            if (errors.Any())
                throw new DomainValidationException(string.Join("; ", errors));
        }
    }
}