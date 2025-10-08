using Services.DAL.Factory;
using Services.DomainModel.Security.Composite;
using Services.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PatenteService : IPatenteService
    {
        private readonly ILogger _logger;

        public PatenteService(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<Patente> GetAll()
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

        public IEnumerable<Patente> GetByUsuario(Guid idUsuario)
        {
            try
            {
                var usuario = LoginFactory.usuarioRepository.SelectOne(idUsuario);
                return usuario?.GetPatentesAll() ?? Enumerable.Empty<Patente>();
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al obtener patentes del usuario: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }
    }
}
