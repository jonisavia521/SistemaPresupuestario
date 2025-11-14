using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de mapeo AutoMapper para Configuracion
    /// Define las transformaciones entre ConfiguracionDM (dominio) y ConfiguracionDTO (vista)
    /// </summary>
    public class ConfiguracionMappingProfile : Profile
    {
        public ConfiguracionMappingProfile()
        {
            // Mapeo de dominio a DTO (para lectura)
            CreateMap<ConfiguracionDM, ConfiguracionDTO>();

            // Mapeo de DTO a dominio (para escritura)
            // Nota: Se usa ForCtorParam porque ConfiguracionDM tiene constructor con parámetros
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
