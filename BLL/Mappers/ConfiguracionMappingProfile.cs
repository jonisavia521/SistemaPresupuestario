using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de Mapeo AutoMapper para Configuración.
    /// 
    /// Define las transformaciones entre:
    /// - ConfiguracionDM (Model de Dominio): Entidad única de configuración del sistema
    /// - ConfiguracionDTO (Data Transfer Object): Para transferencia entre capas
    /// 
    /// La configuración es un conjunto singular de parámetros del sistema.
    /// Solo existe un registro de configuración activo en toda la aplicación.
    /// </summary>
    public class ConfiguracionMappingProfile : Profile
    {
        /// <summary>
        /// Constructor que define los mapeos bidireccionales para Configuración.
        /// </summary>
        public ConfiguracionMappingProfile()
        {
            /// <summary>
            /// Mapeo de ConfiguracionDM a ConfiguracionDTO (desde BLL a UI).
            /// Los nombres coinciden, AutoMapper realiza el mapeo automático.
            /// </summary>
            CreateMap<ConfiguracionDM, ConfiguracionDTO>();

            /// <summary>
            /// Mapeo de ConfiguracionDTO a ConfiguracionDM (desde UI a BLL).
            /// 
            /// Nota: Utiliza ConstructUsing porque ConfiguracionDM tiene un constructor
            /// especializado que requiere todos los parámetros en un orden específico.
            /// El constructor puede ejecutar validaciones de negocio.
            /// </summary>
            CreateMap<ConfiguracionDTO, ConfiguracionDM>()
                .ConstructUsing(dto => new ConfiguracionDM(
                    dto.RazonSocial,
                    dto.CUIT,
                    dto.TipoIva,
                    dto.Idioma,
                    dto.Direccion,
                    dto.Localidad,
                    dto.IdProvincia,
                    dto.Email,
                    dto.Telefono
                ));
        }
    }
}
