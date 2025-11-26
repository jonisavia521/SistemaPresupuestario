using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de Mapeo AutoMapper para Producto.
    /// 
    /// Define las transformaciones bidireccionales entre:
    /// - ProductoDM (Model de Dominio): Entidad que contiene la lógica de negocio
    /// - ProductoDTO (Data Transfer Object): Objeto para transferir datos entre UI y BLL
    /// 
    /// Nota especial: ProductoDM usa "ID" como nombre de propiedad (mayúsculas),
    /// mientras que ProductoDTO usa "Id" (convención estándar). El mapeo incluye
    /// una transformación de nombres para mantener consistencia en ambas capas.
    /// </summary>
    public class ProductoMappingProfile : Profile
    {
        /// <summary>
        /// Constructor del perfil de mapeo de Producto.
        /// Define todas las reglas de transformación entre ProductoDM y ProductoDTO.
        /// </summary>
        public ProductoMappingProfile()
        {
            /// <summary>
            /// Mapeo de ProductoDM (entidad de dominio) a ProductoDTO (objeto de transferencia).
            /// Se ejecuta cuando se retornan datos desde BLL hacia la UI.
            /// 
            /// Nota: Mapea ProductoDM.ID ? ProductoDTO.Id (conversión de nombres).
            /// </summary>
            CreateMap<ProductoDM, ProductoDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Inhabilitado, opt => opt.MapFrom(src => src.Inhabilitado))
                .ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaAlta))
                .ForMember(dest => dest.UsuarioAlta, opt => opt.MapFrom(src => src.UsuarioAlta))
                .ForMember(dest => dest.PorcentajeIVA, opt => opt.MapFrom(src => src.PorcentajeIVA));

            /// <summary>
            /// Mapeo inverso: ProductoDTO a ProductoDM.
            /// Se ejecuta cuando se envían datos desde la UI hacia BLL.
            /// 
            /// Nota: Mapea ProductoDTO.Id ? ProductoDM.ID (conversión de nombres).
            /// </summary>
            CreateMap<ProductoDTO, ProductoDM>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id));
        }
    }
}
