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
    public class FamiliaService : IFamiliaService
    {
        private readonly ILogger _logger;

        public FamiliaService(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<Familia> GetAll()
        {
            try
            {
                _logger.WriteLog("Obteniendo todas las familias", EventLevel.Informational, string.Empty);
                return LoginFactory.familiaRepository.SelectAll();
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al obtener familias: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }

        public IEnumerable<Familia> GetByUsuario(Guid idUsuario)
        {
            try
            {
                var usuario = LoginFactory.usuarioRepository.SelectOne(idUsuario);
                return usuario?.Permisos.OfType<Familia>() ?? Enumerable.Empty<Familia>();
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error al obtener familias del usuario: {ex.Message}", EventLevel.Error, string.Empty);
                throw;
            }
        }
    }
}
