using AutoMapper;
using BLL.Contracts.Seguridad;
using BLL.DTOs.Seguridad;
using BLL.Exceptions;
using DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Seguridad
{
    /// <summary>
    /// Servicio de negocio para cálculo y gestión de permisos efectivos
    /// </summary>
    public class PermisosBusinessService : IPermisosBusinessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        // Cache temporal para optimizar cálculos repetitivos
        private readonly Dictionary<Guid, List<PermisoEfectivoDto>> _cachePermisosEfectivos;
        private readonly object _cacheLock = new object();

        public PermisosBusinessService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cachePermisosEfectivos = new Dictionary<Guid, List<PermisoEfectivoDto>>();
        }

        public async Task<IEnumerable<PermisoEfectivoDto>> CalcularPermisosEfectivosAsync(Guid usuarioId)
        {
            // Verificar cache temporal
            lock (_cacheLock)
            {
                if (_cachePermisosEfectivos.ContainsKey(usuarioId))
                {
                    _logger.LogDebug($"Permisos efectivos obtenidos desde cache para usuario {usuarioId}");
                    return _cachePermisosEfectivos[usuarioId];
                }
            }

            _logger.LogDebug($"Calculando permisos efectivos para usuario {usuarioId}");

            // Obtener patentes directas
            var patentesDirectas = await GetPatentesDirectasUsuarioAsync(usuarioId);
            
            // Obtener patentes heredadas
            var patentesHeredadas = await GetPatentesHeredadasUsuarioAsync(usuarioId);

            // Combinar y eliminar duplicados
            var permisosEfectivos = new Dictionary<Guid, PermisoEfectivoDto>();

            // Agregar patentes directas
            foreach (var patente in patentesDirectas)
            {
                var permiso = new PermisoEfectivoDto
                {
                    Patente = patente,
                    EsDirecto = true,
                    EsHeredado = false,
                    Origenes = new List<OrigenPermisoDto>
                    {
                        new OrigenPermisoDto
                        {
                            Id = usuarioId,
                            Nombre = "Directo",
                            Tipo = TipoOrigenPermiso.Directo
                        }
                    }
                };
                permisosEfectivos[patente.Id] = permiso;
            }

            // Agregar patentes heredadas (pueden sobrescribir o complementar directas)
            foreach (var patente in patentesHeredadas)
            {
                if (permisosEfectivos.ContainsKey(patente.Id))
                {
                    // Ya existe como directa, marcar también como heredada
                    var permisoExistente = permisosEfectivos[patente.Id];
                    permisoExistente.EsHeredado = true;
                    
                    // Agregar origen de familia si no existe
                    var origenFamilia = new OrigenPermisoDto
                    {
                        Id = Guid.Empty, // TODO: Obtener ID de familia origen
                        Nombre = patente.FamiliaOrigen ?? "Familia",
                        Tipo = TipoOrigenPermiso.Familia
                    };
                    
                    if (!permisoExistente.Origenes.Any(o => o.Tipo == TipoOrigenPermiso.Familia && o.Nombre == origenFamilia.Nombre))
                    {
                        permisoExistente.Origenes.Add(origenFamilia);
                    }
                }
                else
                {
                    // Solo heredada
                    var permiso = new PermisoEfectivoDto
                    {
                        Patente = patente,
                        EsDirecto = false,
                        EsHeredado = true,
                        Origenes = new List<OrigenPermisoDto>
                        {
                            new OrigenPermisoDto
                            {
                                Id = Guid.Empty, // TODO: Obtener ID de familia origen
                                Nombre = patente.FamiliaOrigen ?? "Familia",
                                Tipo = TipoOrigenPermiso.Familia
                            }
                        }
                    };
                    permisosEfectivos[patente.Id] = permiso;
                }
            }

            var resultado = permisosEfectivos.Values.ToList();

            // Guardar en cache temporal
            lock (_cacheLock)
            {
                _cachePermisosEfectivos[usuarioId] = resultado;
            }

            _logger.LogInfo($"Permisos efectivos calculados para usuario {usuarioId}: {resultado.Count} permisos únicos");
            return resultado;
        }

        public async Task<IEnumerable<PatenteDto>> GetPatentesDirectasUsuarioAsync(Guid usuarioId)
        {
            var patentes = await _unitOfWork.UsuariosSecurity.GetPatentesAsignadasAsync(usuarioId);
            return patentes.Select(p => _mapper.Map<PatenteDto>(p));
        }

        public async Task<IEnumerable<PatenteDto>> GetPatentesHeredadasUsuarioAsync(Guid usuarioId)
        {
            var patentesHeredadas = await _unitOfWork.Patentes.GetPatentesHeredadasAsync(usuarioId);
            var resultado = new List<PatenteDto>();

            foreach (var (patente, familiaOrigen) in patentesHeredadas)
            {
                var patenteDto = _mapper.Map<PatenteDto>(patente);
                patenteDto.EsHeredada = true;
                patenteDto.FamiliaOrigen = familiaOrigen?.Nombre;
                resultado.Add(patenteDto);
            }

            return resultado;
        }

        public async Task<IEnumerable<PatenteDto>> GetAllPatentesWithAssignmentStatusAsync(Guid usuarioId)
        {
            // Obtener todas las patentes
            var todasPatentes = await _unitOfWork.Patentes.GetAllAsync();
            
            // Obtener patentes directas del usuario
            var patentesDirectas = await _unitOfWork.UsuariosSecurity.GetPatentesAsignadasAsync(usuarioId);
            var patentesDirectasIds = patentesDirectas.Select(p => p.IdPatente).ToHashSet();
            
            // Obtener patentes heredadas
            var patentesHeredadas = await _unitOfWork.Patentes.GetPatentesHeredadasAsync(usuarioId);
            var patentesHeredadasIds = patentesHeredadas.Select(ph => ph.patente.IdPatente).ToHashSet();

            var resultado = new List<PatenteDto>();
            foreach (var patente in todasPatentes)
            {
                var patenteDto = _mapper.Map<PatenteDto>(patente);
                patenteDto.AsignadaDirectamente = patentesDirectasIds.Contains(patente.IdPatente);
                patenteDto.EsHeredada = patentesHeredadasIds.Contains(patente.IdPatente);
                
                if (patenteDto.EsHeredada)
                {
                    var familiaOrigen = patentesHeredadas
                        .FirstOrDefault(ph => ph.patente.IdPatente == patente.IdPatente)
                        .familiaOrigen;
                    patenteDto.FamiliaOrigen = familiaOrigen?.Nombre;
                }
                
                resultado.Add(patenteDto);
            }

            return resultado;
        }

        public async Task AsignarFamiliasAsync(Guid usuarioId, IEnumerable<Guid> familiasIds, string versionConcurrencia)
        {
            _logger.LogInfo($"Asignando familias a usuario {usuarioId}: {string.Join(", ", familiasIds ?? new List<Guid>())}");

            // Verificar que el usuario existe
            var usuario = await _unitOfWork.UsuariosSecurity.GetByIdAsync(usuarioId);
            if (usuario == null)
                throw new NotFoundException($"Usuario con ID {usuarioId} no encontrado", usuarioId, "Usuario");

            // Verificar concurrencia
            var versionActual = usuario.timestamp != null ? Convert.ToBase64String(usuario.timestamp) : null;
            if (versionActual != versionConcurrencia)
                throw new ConcurrencyException("El usuario ha sido modificado por otro usuario", usuarioId, "Usuario", versionConcurrencia, versionActual);

            _unitOfWork.BeginTransaction();
            try
            {
                await _unitOfWork.UsuariosSecurity.AsignarFamiliasAsync(usuarioId, familiasIds);
                await Task.Run(() => _unitOfWork.SaveChanges());
                _unitOfWork.Commit();

                // Limpiar cache
                InvalidatePermissionsCache(usuarioId);
                
                _logger.LogInfo($"Familias asignadas exitosamente a usuario {usuarioId}");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError($"Error al asignar familias a usuario {usuarioId}", ex);
                throw;
            }
        }

        public async Task AsignarPatentesAsync(Guid usuarioId, IEnumerable<Guid> patentesIds, string versionConcurrencia)
        {
            _logger.LogInfo($"Asignando patentes a usuario {usuarioId}: {string.Join(", ", patentesIds ?? new List<Guid>())}");

            // Verificar que el usuario existe
            var usuario = await _unitOfWork.UsuariosSecurity.GetByIdAsync(usuarioId);
            if (usuario == null)
                throw new NotFoundException($"Usuario con ID {usuarioId} no encontrado", usuarioId, "Usuario");

            // Verificar concurrencia
            var versionActual = usuario.timestamp != null ? Convert.ToBase64String(usuario.timestamp) : null;
            if (versionActual != versionConcurrencia)
                throw new ConcurrencyException("El usuario ha sido modificado por otro usuario", usuarioId, "Usuario", versionConcurrencia, versionActual);

            _unitOfWork.BeginTransaction();
            try
            {
                await _unitOfWork.UsuariosSecurity.AsignarPatentesAsync(usuarioId, patentesIds);
                await Task.Run(() => _unitOfWork.SaveChanges());
                _unitOfWork.Commit();

                // Limpiar cache
                InvalidatePermissionsCache(usuarioId);
                
                _logger.LogInfo($"Patentes asignadas exitosamente a usuario {usuarioId}");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError($"Error al asignar patentes a usuario {usuarioId}", ex);
                throw;
            }
        }

        public async Task<bool> ValidarUsuarioTienePermisosAsync(Guid usuarioId, IEnumerable<Guid> familiasIds, IEnumerable<Guid> patentesIds)
        {
            // Si tiene patentes directas, tiene permisos
            if (patentesIds?.Any() == true)
                return true;

            // Si no tiene familias, no tiene permisos
            if (familiasIds?.Any() != true)
                return false;

            // Verificar si las familias asignadas tienen patentes
            foreach (var familiaId in familiasIds)
            {
                var patentesEnFamilia = await _unitOfWork.Familias.GetPatentesDirectasAsync(familiaId);
                if (patentesEnFamilia.Any())
                    return true;

                // TODO: Verificar familias hijas recursivamente
            }

            return false;
        }

        public async Task<PermissionCountInfo> GetPermissionCountsAsync(Guid usuarioId)
        {
            var (familiasDirectas, patentesDirectas) = await _unitOfWork.UsuariosSecurity.GetPermissionCountsAsync(usuarioId);
            
            // TODO: Calcular patentes heredadas y efectivas
            var permisosEfectivos = await CalcularPermisosEfectivosAsync(usuarioId);
            var patentesEfectivasUnicas = permisosEfectivos.Count();
            var patentesHeredadas = permisosEfectivos.Count(p => p.EsHeredado && !p.EsDirecto);

            return new PermissionCountInfo
            {
                PatentesDirectas = patentesDirectas,
                FamiliasDirectas = familiasDirectas,
                PatentesHeredadas = patentesHeredadas,
                PermisosEfectivosUnicos = patentesEfectivasUnicas
            };
        }

        /// <summary>
        /// Invalida el cache de permisos para un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        private void InvalidatePermissionsCache(Guid usuarioId)
        {
            lock (_cacheLock)
            {
                if (_cachePermisosEfectivos.ContainsKey(usuarioId))
                {
                    _cachePermisosEfectivos.Remove(usuarioId);
                    _logger.LogDebug($"Cache de permisos invalidado para usuario {usuarioId}");
                }
            }
        }
    }
}