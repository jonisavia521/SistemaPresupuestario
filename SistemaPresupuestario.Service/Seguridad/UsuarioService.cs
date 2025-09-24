using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SistemaPresupuestario.BLL.DTOs;
using SistemaPresupuestario.BLL.Profiles;
using SistemaPresupuestario.BLL.Services.Implementations;
using SistemaPresupuestario.DAL.UnitOfWork;
using SistemaPresupuestario.DomainModel.Seguridad;

namespace SistemaPresupuestario.Service.Seguridad
{
    /// <summary>
    /// Service facade implementation for user management
    /// Coordinates between UI, BLL, and DAL layers
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PasswordHasher _passwordHasher;

        public UsuarioService()
        {
            _unitOfWork = new UnitOfWork();
            _mapper = SeguridadProfile.CreateMapper();
            _passwordHasher = new PasswordHasher();
        }

        public UsuarioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = SeguridadProfile.CreateMapper();
            _passwordHasher = new PasswordHasher();
        }

        public IEnumerable<UsuarioDto> GetUsers(string searchText = null)
        {
            try
            {
                var usuarios = _unitOfWork.Usuarios.GetUsersFiltered(searchText, 1, 100, out int totalCount);
                return usuarios.Select(u => _mapper.Map<UsuarioDto>(u)).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving users", ex);
            }
        }

        public UsuarioDto GetUserById(Guid userId)
        {
            try
            {
                var usuario = _unitOfWork.Usuarios.GetByIdWithRelations(userId);
                return usuario != null ? _mapper.Map<UsuarioDto>(usuario) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving user {userId}", ex);
            }
        }

        public UsuarioDto CreateUser(UsuarioEditDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            try
            {
                _unitOfWork.BeginTransaction();

                // Check username uniqueness
                if (_unitOfWork.Usuarios.ExistsUsername(userDto.NombreUsuario))
                {
                    throw new InvalidOperationException($"Username '{userDto.NombreUsuario}' already exists");
                }

                // Hash password
                var hashedPassword = _passwordHasher.HashPassword(userDto.Clave, out string salt);

                // Map to domain entity
                var usuario = _mapper.Map<Usuario>(userDto);
                usuario.ClaveHash = hashedPassword;
                usuario.Salt = salt;

                // Add user with relations
                _unitOfWork.Usuarios.AddUserWithRelations(usuario, userDto.FamiliasAsignadasIds, userDto.PatentesAsignadasIds);

                _unitOfWork.CommitTransaction();

                return _mapper.Map<UsuarioDto>(usuario);
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public UsuarioDto UpdateUser(UsuarioEditDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            try
            {
                _unitOfWork.BeginTransaction();

                // Check username uniqueness (excluding current user)
                if (_unitOfWork.Usuarios.ExistsUsername(userDto.NombreUsuario, userDto.Id))
                {
                    throw new InvalidOperationException($"Username '{userDto.NombreUsuario}' already exists");
                }

                // Map to domain entity
                var usuario = _mapper.Map<Usuario>(userDto);

                // Hash password if changing
                if (userDto.CambiarClave && !string.IsNullOrWhiteSpace(userDto.Clave))
                {
                    var hashedPassword = _passwordHasher.HashPassword(userDto.Clave, out string salt);
                    usuario.ClaveHash = hashedPassword;
                    usuario.Salt = salt;
                }

                // Update user with relations
                _unitOfWork.Usuarios.UpdateUserWithRelations(usuario, userDto.FamiliasAsignadasIds, userDto.PatentesAsignadasIds);

                _unitOfWork.CommitTransaction();

                return _mapper.Map<UsuarioDto>(usuario);
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public void DeleteUser(Guid userId)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var usuario = _unitOfWork.Usuarios.GetById(userId);
                if (usuario == null)
                    throw new InvalidOperationException($"User {userId} not found");

                _unitOfWork.Usuarios.Remove(usuario);
                _unitOfWork.CommitTransaction();
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public IEnumerable<FamiliaDto> GetAvailableFamilies()
        {
            try
            {
                var familias = _unitOfWork.Familias.GetAll();
                return familias.Select(f => _mapper.Map<FamiliaDto>(f)).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving families", ex);
            }
        }

        public IEnumerable<PatenteDto> GetAvailablePatents()
        {
            try
            {
                var patentes = _unitOfWork.Patentes.GetAll();
                return patentes.Select(p => _mapper.Map<PatenteDto>(p)).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving patents", ex);
            }
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }
    }
}