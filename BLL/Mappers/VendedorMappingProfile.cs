using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de mapeo AutoMapper para Vendedor
    /// Define las transformaciones entre entidades de dominio y DTOs
    /// </summary>
    public class VendedorMappingProfile : Profile
    {
        public VendedorMappingProfile()
        {
            // Mapeo de VendedorDM (Dominio) a VendedorDTO (Vista)
            CreateMap<VendedorDM, VendedorDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CodigoVendedor, opt => opt.MapFrom(src => src.CodigoVendedor))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.CUIT, opt => opt.MapFrom(src => src.CUIT))
                .ForMember(dest => dest.PorcentajeComision, opt => opt.MapFrom(src => src.PorcentajeComision))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.Telefono))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Activo))
                .ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaAlta))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => src.FechaModificacion));

            // Mapeo inverso
            CreateMap<VendedorDTO, VendedorDM>();
        }
    }
}
