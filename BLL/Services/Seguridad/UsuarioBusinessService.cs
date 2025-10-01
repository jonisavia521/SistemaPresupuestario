using AutoMapper;
using BLL.Contracts.Seguridad;
using BLL.DTOs.Seguridad;
using BLL.Exceptions;
using DAL.Contracts;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Seguridad
{
    /// <summary>
    /// Servicio de negocio para gestión de usuarios con permisos
    /// DECISIÓN: Bypass de Service Layer - llamado directamente desde UI
    /// </summary>
    public class UsuarioBusinessService : IUsuarioBusinessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UsuarioBusinessService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var usuarios = await _unitOfWork.UsuariosSecurity.GetAllAsync();
            var userDtos = new List<UserDto>();

            foreach (var usuario in usuarios)
            {
                var userDto = _mapper.Map<UserDto>(usuario);
                
                // Calcular conteos de permisos
                var (familiasDirectas, patentesDirectas) = await _unitOfWork.UsuariosSecurity
                    .GetPermissionCountsAsync(usuario.IdUsuario);
                
                userDto.CantidadFamiliasDirectas = familiasDirectas;
                userDto.CantidadPatentesDirectas = patentesDirectas;
                userDto.VersionConcurrencia = usuario.timestamp != null ? Convert.ToBase64String(usuario.timestamp) : null;
                
                // TODO: Calcular permisos efectivos (se implementará en PermisosBusinessService)
                userDto.CantidadPermisosEfectivos = patentesDirectas; // Placeholder
                
                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<UserEditDto> GetUserForEditAsync(Guid id)
        {
            var usuario = await _unitOfWork.UsuariosSecurity.GetWithPermissionsAsync(id);
            if (usuario == null)
                throw new NotFoundException($"Usuario con ID {id} no encontrado", id, "Usuario");

            var userDto = _mapper.Map<UserEditDto>(usuario);
            
            // Cargar familias y patentes asignadas
            userDto.FamiliasAsignadas = usuario.Usuario_Familia?.Select(uf => uf.IdFamilia).ToList() ?? new List<Guid>();
            userDto.PatentesAsignadas = usuario.Usuario_Patente?.Select(up => up.IdPatente).ToList() ?? new List<Guid>();
            userDto.VersionConcurrencia = usuario.timestamp != null ? Convert.ToBase64String(usuario.timestamp) : null;
            
            return userDto;
        }

        public async Task<Guid> CreateUserAsync(UserEditDto userDto)
        {
            // Validaciones
            await ValidateUserDto(userDto, isCreate: true);

            // Verificar que el nombre de usuario no esté duplicado
            var exists = await _unitOfWork.UsuariosSecurity.ExistsUsernameAsync(userDto.Usuario);
            if (exists)
                throw new DomainValidationException($"El nombre de usuario '{userDto.Usuario}' ya existe", "Usuario", "USERNAME_EXISTS");

            // Hash de la contraseña
            if (string.IsNullOrEmpty(userDto.Clave))
                throw new DomainValidationException("La contraseña es obligatoria para usuarios nuevos", "Clave", "PASSWORD_REQUIRED");

            var hashedPassword = _passwordHasher.HashPassword(userDto.Clave);

            // Crear dominio y entidad
            var usuarioDomain = new UsuarioDomain(userDto.Nombre.Trim(), userDto.Usuario.Trim(), hashedPassword);
            var usuarioEntity = _mapper.Map<DAL.Implementation.EntityFramework.Usuario>(usuarioDomain);

            _unitOfWork.BeginTransaction();
            try
            {
                // Agregar usuario
                _unitOfWork.UsuariosSecurity.Add(usuarioEntity);
                await Task.Run(() => _unitOfWork.SaveChanges()); // SaveChanges sync en EF6

                var usuarioId = usuarioEntity.IdUsuario;

                // Asignar familias y patentes
                if (userDto.FamiliasAsignadas?.Any() == true)
                {
                    await _unitOfWork.UsuariosSecurity.AsignarFamiliasAsync(usuarioId, userDto.FamiliasAsignadas);
                }

                if (userDto.PatentesAsignadas?.Any() == true)
                {
                    await _unitOfWork.UsuariosSecurity.AsignarPatentesAsync(usuarioId, userDto.PatentesAsignadas);
                }

                await Task.Run(() => _unitOfWork.SaveChanges());
                _unitOfWork.Commit();

                return usuarioId;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task UpdateUserAsync(UserEditDto userDto)
        {
            // Validaciones
            await ValidateUserDto(userDto, isCreate: false);

            if (userDto.Id == Guid.Empty)
                throw new DomainValidationException("ID de usuario requerido para actualización", "Id", "ID_REQUIRED");

            // Obtener usuario actual
            var usuarioActual = await _unitOfWork.UsuariosSecurity.GetByIdAsync(userDto.Id);
            if (usuarioActual == null)
                throw new NotFoundException($"Usuario con ID {userDto.Id} no encontrado", userDto.Id, "Usuario");

            // Verificar concurrencia
            var versionActual = usuarioActual.timestamp != null ? Convert.ToBase64String(usuarioActual.timestamp) : null;
            if (versionActual != userDto.VersionConcurrencia)
                throw new ConcurrencyException("El usuario ha sido modificado por otro usuario", userDto.Id, "Usuario", userDto.VersionConcurrencia, versionActual);

            // Verificar que el nombre de usuario no esté duplicado
            var exists = await _unitOfWork.UsuariosSecurity.ExistsUsernameAsync(userDto.Usuario, userDto.Id);
            if (exists)
                throw new DomainValidationException($"El nombre de usuario '{userDto.Usuario}' ya existe", "Usuario", "USERNAME_EXISTS");

            _unitOfWork.BeginTransaction();
            try
            {
                // Actualizar datos básicos
                usuarioActual.Nombre = userDto.Nombre.Trim();
                usuarioActual.Usuario1 = userDto.Usuario.Trim();

                // Cambiar contraseña si se especifica
                if (userDto.CambiarClave && !string.IsNullOrEmpty(userDto.Clave))
                {
                    usuarioActual.Clave = _passwordHasher.HashPassword(userDto.Clave);
                }

                _unitOfWork.UsuariosSecurity.Update(usuarioActual);

                // Actualizar asignaciones de familias y patentes
                if (userDto.FamiliasAsignadas != null)
                {
                    await _unitOfWork.UsuariosSecurity.AsignarFamiliasAsync(userDto.Id, userDto.FamiliasAsignadas);
                }

                if (userDto.PatentesAsignadas != null)
                {
                    await _unitOfWork.UsuariosSecurity.AsignarPatentesAsync(userDto.Id, userDto.PatentesAsignadas);
                }

                await Task.Run(() => _unitOfWork.SaveChanges());
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task DeleteUserAsync(Guid id, string versionConcurrencia)
        {
            var usuario = await _unitOfWork.UsuariosSecurity.GetByIdAsync(id);
            if (usuario == null)
                throw new NotFoundException($"Usuario con ID {id} no encontrado", id, "Usuario");

            // Verificar concurrencia
            var versionActual = usuario.timestamp != null ? Convert.ToBase64String(usuario.timestamp) : null;
            if (versionActual != versionConcurrencia)
                throw new ConcurrencyException("El usuario ha sido modificado por otro usuario", id, "Usuario", versionConcurrencia, versionActual);

            _unitOfWork.BeginTransaction();
            try
            {
                // TODO: Decidir si es eliminación física o lógica
                // Por ahora implementamos eliminación física
                _unitOfWork.UsuariosSecurity.Delete(usuario);
                await Task.Run(() => _unitOfWork.SaveChanges());
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task ChangePasswordAsync(Guid id, string nuevaClave, string versionConcurrencia)
        {
            if (string.IsNullOrWhiteSpace(nuevaClave))
                throw new DomainValidationException("La nueva contraseña no puede estar vacía", "NuevaClave", "PASSWORD_REQUIRED");

            if (nuevaClave.Length < 6)
                throw new DomainValidationException("La contraseña debe tener al menos 6 caracteres", "NuevaClave", "PASSWORD_TOO_SHORT");

            var usuario = await _unitOfWork.UsuariosSecurity.GetByIdAsync(id);
            if (usuario == null)
                throw new NotFoundException($"Usuario con ID {id} no encontrado", id, "Usuario");

            // Verificar concurrencia
            var versionActual = usuario.timestamp != null ? Convert.ToBase64String(usuario.timestamp) : null;
            if (versionActual != versionConcurrencia)
                throw new ConcurrencyException("El usuario ha sido modificado por otro usuario", id, "Usuario", versionConcurrencia, versionActual);

            _unitOfWork.BeginTransaction();
            try
            {
                usuario.Clave = _passwordHasher.HashPassword(nuevaClave);
                _unitOfWork.UsuariosSecurity.Update(usuario);
                await Task.Run(() => _unitOfWork.SaveChanges());
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string nombreUsuario, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return false;

            return !await _unitOfWork.UsuariosSecurity.ExistsUsernameAsync(nombreUsuario.Trim(), excludeId);
        }

        public async Task<PagedResult<UserDto>> GetUsersPagedAsync(string filtroNombre = null, string filtroUsuario = null, bool soloActivos = true, int pagina = 1, int tamanoPagina = 50)
        {
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1) tamanoPagina = 50;
            if (tamanoPagina > 100) tamanoPagina = 100;

            var skip = (pagina - 1) * tamanoPagina;
            
            var (usuarios, total) = await _unitOfWork.UsuariosSecurity.GetPagedAsync(
                filtroNombre?.Trim(), filtroUsuario?.Trim(), soloActivos, skip, tamanoPagina);

            var userDtos = new List<UserDto>();
            foreach (var usuario in usuarios)
            {
                var userDto = _mapper.Map<UserDto>(usuario);
                
                // Calcular conteos de permisos
                var (familiasDirectas, patentesDirectas) = await _unitOfWork.UsuariosSecurity
                    .GetPermissionCountsAsync(usuario.IdUsuario);
                
                userDto.CantidadFamiliasDirectas = familiasDirectas;
                userDto.CantidadPatentesDirectas = patentesDirectas;
                userDto.VersionConcurrencia = usuario.timestamp != null ? Convert.ToBase64String(usuario.timestamp) : null;
                userDto.CantidadPermisosEfectivos = patentesDirectas; // Placeholder
                
                userDtos.Add(userDto);
            }

            var totalPages = (int)Math.Ceiling((double)total / tamanoPagina);

            return new PagedResult<UserDto>
            {
                Data = userDtos,
                TotalRecords = total,
                PageNumber = pagina,
                PageSize = tamanoPagina,
                TotalPages = totalPages,
                HasNextPage = pagina < totalPages,
                HasPreviousPage = pagina > 1
            };
        }

        private async Task ValidateUserDto(UserEditDto userDto, bool isCreate)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            if (string.IsNullOrWhiteSpace(userDto.Nombre))
                throw new DomainValidationException("El nombre es obligatorio", "Nombre", "NAME_REQUIRED");

            if (string.IsNullOrWhiteSpace(userDto.Usuario))
                throw new DomainValidationException("El nombre de usuario es obligatorio", "Usuario", "USERNAME_REQUIRED");

            if (userDto.Nombre.Trim().Length < 2)
                throw new DomainValidationException("El nombre debe tener al menos 2 caracteres", "Nombre", "NAME_TOO_SHORT");

            if (userDto.Usuario.Trim().Length < 3)
                throw new DomainValidationException("El nombre de usuario debe tener al menos 3 caracteres", "Usuario", "USERNAME_TOO_SHORT");

            // Validar formato de nombre de usuario
            if (!System.Text.RegularExpressions.Regex.IsMatch(userDto.Usuario.Trim(), @"^[a-zA-Z0-9_]+$"))
                throw new DomainValidationException("El nombre de usuario solo puede contener letras, números y guiones bajos", "Usuario", "USERNAME_INVALID_FORMAT");

            // Validar contraseña para creación o cambio
            if (isCreate || userDto.CambiarClave)
            {
                if (string.IsNullOrWhiteSpace(userDto.Clave))
                    throw new DomainValidationException("La contraseña es obligatoria", "Clave", "PASSWORD_REQUIRED");

                if (userDto.Clave.Length < 6)
                    throw new DomainValidationException("La contraseña debe tener al menos 6 caracteres", "Clave", "PASSWORD_TOO_SHORT");

                if (userDto.Clave != userDto.ConfirmarClave)
                    throw new DomainValidationException("Las contraseñas no coinciden", "ConfirmarClave", "PASSWORD_MISMATCH");
            }

            await Task.CompletedTask; // Para mantener el método async
        }
    }
}