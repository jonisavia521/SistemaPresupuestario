using DAL.Implementation.EntityFramework;
using DAL.Infrastructure;
using DomainModel.Contract;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Implementation.Repository
{
    /// <summary>
    /// Repositorio de acceso a datos para Configuracion
    /// Implementa las operaciones de persistencia específicas de Configuracion
    /// Esta tabla siempre tendrá un único registro
    /// </summary>
    public class ConfiguracionRepository : Repository<ConfiguracionDM>, IConfiguracionRepository
    {
        public ConfiguracionRepository(SistemaPresupuestario context) : base(context)
        {
        }

        public ConfiguracionDM ObtenerConfiguracion()
        {
            // Obtener el primer (y único) registro
            var configuracionEF = _context.Configuracion.FirstOrDefault();

            if (configuracionEF == null)
                return null;

            // Convertir de entidad EF a entidad de dominio
            return MapearADominio(configuracionEF);
        }

        public bool ExisteConfiguracion()
        {
            return _context.Configuracion.Any();
        }

        public new ConfiguracionDM GetById(Guid id)
        {
            var configuracionEF = _context.Configuracion.Find(id);
            if (configuracionEF == null)
                return null;

            return MapearADominio(configuracionEF);
        }

        // Sobrescribir métodos base para usar el mapeo personalizado
        public new IEnumerable<ConfiguracionDM> GetAll()
        {
            var configuracionesEF = _context.Configuracion.ToList();
            return configuracionesEF.Select(MapearADominio);
        }

        public new void Add(ConfiguracionDM entidad)
        {
            var configuracionEF = MapearAEntityFramework(entidad);
            _context.Configuracion.Add(configuracionEF);
        }

        public new void Update(ConfiguracionDM entidad)
        {
            var configuracionEF = MapearAEntityFramework(entidad);
            
            var existente = _context.Configuracion.Find(entidad.Id);
            if (existente != null)
            {
                // Actualizar propiedades
                _context.Entry(existente).CurrentValues.SetValues(configuracionEF);
            }
        }

        public new void Delete(ConfiguracionDM entidad)
        {
            var configuracionEF = _context.Configuracion.Find(entidad.Id);
            if (configuracionEF != null)
            {
                _context.Configuracion.Remove(configuracionEF);
            }
        }

        // ==================== MAPEOS ENTRE ENTIDADES ====================

        /// <summary>
        /// Convierte de entidad EF (Configuracion) a entidad de dominio (ConfiguracionDM)
        /// </summary>
        private ConfiguracionDM MapearADominio(Configuracion configuracionEF)
        {
            // Usar el constructor de carga desde BD
            return new ConfiguracionDM(
                configuracionEF.Id,
                configuracionEF.RazonSocial,
                configuracionEF.CUIT,
                configuracionEF.TipoIva,
                configuracionEF.Idioma,
                configuracionEF.FechaAlta,
                configuracionEF.FechaModificacion,
                configuracionEF.Direccion,
                configuracionEF.Localidad,
                configuracionEF.IdProvincia,
                configuracionEF.Email,
                configuracionEF.Telefono
            );
        }

        /// <summary>
        /// Convierte de entidad de dominio (ConfiguracionDM) a entidad EF (Configuracion)
        /// </summary>
        private Configuracion MapearAEntityFramework(ConfiguracionDM dominio)
        {
            return new Configuracion
            {
                Id = dominio.Id,
                RazonSocial = dominio.RazonSocial,
                CUIT = dominio.CUIT,
                TipoIva = dominio.TipoIva,
                Direccion = dominio.Direccion,
                Localidad = dominio.Localidad,
                IdProvincia = dominio.IdProvincia,
                Email = dominio.Email,
                Telefono = dominio.Telefono,
                Idioma = dominio.Idioma,
                FechaAlta = dominio.FechaAlta,
                FechaModificacion = dominio.FechaModificacion
            };
        }
    }
}
