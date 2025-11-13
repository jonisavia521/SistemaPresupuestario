using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de mapeo AutoMapper para Provincia
    /// Define las transformaciones entre entidades de dominio y DTOs
    /// </summary>
    public class ProvinciaMappingProfile : Profile
    {
        public ProvinciaMappingProfile()
        {
            // Mapeo de ProvinciaDM (Dominio) a ProvinciaDTO (Vista)
            CreateMap<ProvinciaDM, ProvinciaDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CodigoAFIP, opt => opt.MapFrom(src => src.CodigoAFIP))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre));

            // Mapeo inverso
            CreateMap<ProvinciaDTO, ProvinciaDM>()
                .ConstructUsing(dto => new ProvinciaDM(
                    dto.Id,
                    dto.CodigoAFIP,
                    dto.Nombre
                ));
        }
    }
}
