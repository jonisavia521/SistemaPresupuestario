using Services.BLL.Contracts;
using Services.DAL.Contracts;
using Services.DAL.Factory;
using Services.DomainModel.Security.Composite;
using Services.Services.Contracts;
using Services.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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


        public IEnumerable<Familia> GetAllFamilias()
        {
            try
            {
                _logger.WriteLog("Obteniendo todas las familias", EventLevel.Informational, string.Empty);

                // Obtener familias raíz (sin padres) para mostrar jerarquía completa
                // Esto depende de cómo quieras mostrarlas en la UI
                return LoginFactory.familiaRepository.SelectAll();
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al obtener familias: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }

        public IEnumerable<Patente> GetAllPatentes()
        {
            try
            {
                _logger.WriteLog("Obteniendo todas las patentes", EventLevel.Informational, string.Empty);
                return LoginFactory.patenteRepository.SelectAll();
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al obtener patentes: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }

        public bool SavePermisos(Usuario usuario)
        {
            try
            {
                _logger.WriteLog($"Guardando permisos para usuario: {usuario.User}", EventLevel.Informational, string.Empty);

                // Guardar familias
                LoginFactory.usuarioFamiliaRepository.Add(usuario);

                // Guardar patentes
                LoginFactory.usuarioPatenteRepository.Add(usuario);

                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al guardar permisos: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }
    }
}