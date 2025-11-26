using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de Mapeo AutoMapper para Vendedor.
    /// 
    /// Define las transformaciones bidireccionales entre:
    /// - VendedorDM (Model de Dominio): Entidad que contiene la lógica de negocio y validaciones
    /// - VendedorDTO (Data Transfer Object): Objeto utilizado para transferir datos entre UI y BLL
    /// 
    /// AutoMapper utiliza este perfil para generar automáticamente código de conversión,
    /// evitando la escritura manual de mapeos repetitivos y propensos a errores.
    /// </summary>
    public class VendedorMappingProfile : Profile
    {
        /// <summary>
        /// Constructor del perfil de mapeo de Vendedor.
        /// Define todas las reglas de transformación entre VendedorDM y VendedorDTO.
        /// </summary>
        public VendedorMappingProfile()
        {
            /// <summary>
            /// Mapeo de VendedorDM (entidad de dominio) a VendedorDTO (objeto de transferencia).
            /// Se ejecuta cuando se retornan datos desde BLL hacia la UI.
            /// </summary>
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

            /// <summary>
            /// Mapeo inverso: VendedorDTO a VendedorDM.
            /// Generalmente NO se utiliza porque VendedorDM tiene constructores específicos
            /// que ejecutan validaciones de negocio. Se incluye por completitud arquitectónica.
            /// </summary>
            CreateMap<VendedorDTO, VendedorDM>();
        }
    }
}
