using AutoMapper;
using BLL.Contracts;
using BLL.DTOs;
using DomainModel.Contract;
using DomainModel.Domain;
using System;
using System.Linq;

namespace BLL.Services
{
    /// <summary>
    /// Servicio de lógica de negocio para Configuración
    /// Coordina operaciones entre UI y DAL, aplicando reglas de negocio
    /// Esta entidad siempre tiene un único registro
    /// </summary>
    public class ConfiguracionService : IConfiguracionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguracionRepository _configuracionRepository;
        private readonly IMapper _mapper;

        public ConfiguracionService(
            IUnitOfWork unitOfWork,
            IConfiguracionRepository configuracionRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuracionRepository = configuracionRepository;
            _mapper = mapper;
        }

        public ConfiguracionDTO ObtenerConfiguracion()
        {
            var entidad = _configuracionRepository.ObtenerConfiguracion();
            return _mapper.Map<ConfiguracionDTO>(entidad);
        }

        public bool ExisteConfiguracion()
        {
            return _configuracionRepository.ExisteConfiguracion();
        }

        public void GuardarConfiguracion(ConfiguracionDTO dto)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                // Verificar si existe configuración
                var entidadExistente = _configuracionRepository.ObtenerConfiguracion();

                if (entidadExistente == null)
                {
                    // Crear nueva configuración
                    var nuevaEntidad = new ConfiguracionDM(
                        dto.RazonSocial,
                        dto.CUIT,
                        dto.TipoIva,
                        dto.Idioma,
                        dto.Direccion,
                        dto.Localidad,
                        dto.IdProvincia,
                        dto.Email,
                        dto.Telefono
                    );

                    // Validación de negocio
                    nuevaEntidad.ValidarNegocio();

                    // Agregar
                    _configuracionRepository.Add(nuevaEntidad);
                }
                else
                {
                    // Actualizar configuración existente
                    entidadExistente.ActualizarDatos(
                        dto.RazonSocial,
                        dto.CUIT,
                        dto.TipoIva,
                        dto.Idioma,
                        dto.Direccion,
                        dto.Localidad,
                        dto.IdProvincia,
                        dto.Email,
                        dto.Telefono
                    );

                    // Validación de negocio
                    entidadExistente.ValidarNegocio();

                    // Actualizar
                    _configuracionRepository.Update(entidadExistente);
                }

                // Confirmar cambios
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
