using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de mapeo AutoMapper para Producto
    /// Define las transformaciones entre entidades de dominio y DTOs
    /// </summary>
    public class ProductoMappingProfile : Profile
    {
        public ProductoMappingProfile()
        {
            // Mapeo de ProductoDM (Dominio) a ProductoDTO (Vista)
            CreateMap<ProductoDM, ProductoDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Inhabilitado, opt => opt.MapFrom(src => src.Inhabilitado))
                .ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaAlta))
                .ForMember(dest => dest.UsuarioAlta, opt => opt.MapFrom(src => src.UsuarioAlta));

            // Mapeo inverso de ProductoDTO a ProductoDM
            CreateMap<ProductoDTO, ProductoDM>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id));
        }
    }
}
