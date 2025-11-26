using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;
using System.Linq;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de Mapeo AutoMapper para ListaPrecio y sus Detalles.
    /// 
    /// Define las transformaciones entre:
    /// - ListaPrecioDM / ListaPrecioDetalleDM (Modelos de Dominio)
    /// - ListaPrecioDTO / ListaPrecioDetalleDTO (Data Transfer Objects)
    /// 
    /// Maneja la relación padre-hijo (Lista ? Detalles) y sus transformaciones.
    /// </summary>
    public class ListaPrecioMappingProfile : Profile
    {
        /// <summary>
        /// Constructor que define todos los mapeos bidireccionales para Listas de Precios.
        /// </summary>
        public ListaPrecioMappingProfile()
        {
            /// <summary>
            /// Mapeo de ListaPrecioDM a ListaPrecioDTO (desde BLL a UI).
            /// Los nombres de propiedades coinciden, así que AutoMapper hace el mapeo automático.
            /// </summary>
            CreateMap<ListaPrecioDM, ListaPrecioDTO>();

            /// <summary>
            /// Mapeo de ListaPrecioDTO a ListaPrecioDM (desde UI a BLL).
            /// Usa constructor especializado de dominio que contiene validaciones.
            /// Transforma la colección de detalles DTO a modelos de dominio.
            /// </summary>
            CreateMap<ListaPrecioDTO, ListaPrecioDM>()
                .ConstructUsing(dto => new ListaPrecioDM(
                    dto.Id,
                    dto.Codigo,
                    dto.Nombre,
                    dto.Activo,
                    dto.FechaAlta,
                    dto.FechaModificacion,
                    dto.Detalles != null ? dto.Detalles.Select(d => MapToDetalleDomain(d)).ToList() : null,
                    dto.IncluyeIva
                ));

            /// <summary>
            /// Mapeo de ListaPrecioDetalleDM a ListaPrecioDetalleDTO (desde BLL a UI).
            /// Ignora Código y Descripción (se cargan por separado del producto).
            /// </summary>
            CreateMap<ListaPrecioDetalleDM, ListaPrecioDetalleDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.Ignore())
                .ForMember(dest => dest.Descripcion, opt => opt.Ignore());

            /// <summary>
            /// Mapeo de ListaPrecioDetalleDTO a ListaPrecioDetalleDM (desde UI a BLL).
            /// Usa constructor especializado de dominio.
            /// </summary>
            CreateMap<ListaPrecioDetalleDTO, ListaPrecioDetalleDM>()
                .ConstructUsing(dto => new ListaPrecioDetalleDM(
                    dto.Id,
                    dto.IdListaPrecio,
                    dto.IdProducto,
                    dto.Precio
                ));
        }

        /// <summary>
        /// Método auxiliar para mapear detalle DTO a dominio.
        /// Crea instancia usando constructor especializado del modelo de dominio.
        /// </summary>
        private static ListaPrecioDetalleDM MapToDetalleDomain(ListaPrecioDetalleDTO dto)
        {
            return new ListaPrecioDetalleDM(
                dto.Id,
                dto.IdListaPrecio,
                dto.IdProducto,
                dto.Precio
            );
        }
    }
}
