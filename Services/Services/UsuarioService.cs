using Services.BLL.Contracts;
using Services.DAL.Contracts;
using Services.DAL.Factory;
using Services.DomainModel.Security.Composite;
using Services.Services.Contracts;
using Services.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    /// <summary>
    /// Servicio para gestión de usuarios
    /// Coordina operaciones entre UI y DAL, aplicando reglas de negocio
    /// </summary>
    internal class UsuarioService : IUsuarioService
    {
        private readonly ILogger _logger;

        public UsuarioService(ILogger logger)
        {
            _logger = logger;
        }

        // GetAllAsync: Retorna lista simplificada para grillas
        public IEnumerable<Usuario> GetAll()
        {
            try
            {
                _logger.WriteLog("Obteniendo lista de usuarios", EventLevel.Informational, string.Empty);

                // Ejecutar en Task para mantener async pattern
                return LoginFactory.usuarioRepository.SelectAll();
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al obtener usuarios: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }

        // GetByIdAsync: Retorna usuario completo con permisos
        public Usuario GetById(Guid id)
        {
            try
            {
                _logger.WriteLog($"Obteniendo usuario ID: {id}", EventLevel.Informational, string.Empty);

                return LoginFactory.usuarioRepository.SelectOne(id);
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al obtener usuario {id}: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }

        // AddAsync: Crea nuevo usuario con validaciones
        public bool Add(Usuario usuario)
        {
            try
            {
                // VALIDACIONES DE NEGOCIO
                if (string.IsNullOrWhiteSpace(usuario.Nombre))
                    throw new ArgumentException("El nombre es requerido");

                if (string.IsNullOrWhiteSpace(usuario.User))
                    throw new ArgumentException("El usuario es requerido");

                if (string.IsNullOrWhiteSpace(usuario.Password))
                    throw new ArgumentException("La contraseña es requerida");

                _logger.WriteLog($"Creando usuario: {usuario.User}", EventLevel.Informational, string.Empty);

                // La contraseña ya viene hasheada desde el constructor del formulario
                LoginFactory.usuarioRepository.Add(usuario);

                _logger.WriteLog($"Usuario {usuario.User} creado exitosamente", EventLevel.Informational, string.Empty);
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al crear usuario: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }

        public bool Update(Usuario usuario)
        {
            try
            {
                _logger.WriteLog($"Actualizando usuario: {usuario.User}", EventLevel.Informational, string.Empty);

                LoginFactory.usuarioRepository.Update(usuario);
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al actualizar usuario: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                _logger.WriteLog($"Eliminando usuario ID: {id}", EventLevel.Informational, string.Empty);

                LoginFactory.usuarioRepository.Delete(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al eliminar usuario: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }
    }
}