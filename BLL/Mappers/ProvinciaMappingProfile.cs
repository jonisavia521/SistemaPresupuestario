using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de Mapeo AutoMapper para Provincia.
    /// 
    /// Define las transformaciones entre:
    /// - ProvinciaDM (Model de Dominio): Entidad de datos de referencia
    /// - ProvinciaDTO (Data Transfer Object): Para transferencia entre capas
    /// 
    /// Las provincias son datos maestros de solo lectura en la mayoría de casos,
    /// aunque el mapeo inverso se proporciona por completitud arquitectónica.
    /// </summary>
    public class ProvinciaMappingProfile : Profile
    {
        /// <summary>
        /// Constructor que define los mapeos bidireccionales para Provincias.
        /// </summary>
        public ProvinciaMappingProfile()
        {
            /// <summary>
            /// Mapeo de ProvinciaDM a ProvinciaDTO (desde BLL a UI).
            /// Los nombres coinciden, se mapean explícitamente para claridad.
            /// </summary>
            CreateMap<ProvinciaDM, ProvinciaDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CodigoAFIP, opt => opt.MapFrom(src => src.CodigoAFIP))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre));

            /// <summary>
            /// Mapeo de ProvinciaDTO a ProvinciaDM (desde UI a BLL).
            /// Usa constructor especializado del modelo de dominio.
            /// </summary>
            CreateMap<ProvinciaDTO, ProvinciaDM>()
                .ConstructUsing(dto => new ProvinciaDM(
                    dto.Id,
                    dto.CodigoAFIP,
                    dto.Nombre
                ));
        }
    }
}
