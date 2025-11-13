using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;
using System.Linq;

namespace BLL.Mappers
{
    public class ListaPrecioMappingProfile : Profile
    {
        public ListaPrecioMappingProfile()
        {
            // Mapeo de ListaPrecioDM a ListaPrecioDTO
            CreateMap<ListaPrecioDM, ListaPrecioDTO>();

            // Mapeo de ListaPrecioDTO a ListaPrecioDM
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

            // Mapeo de ListaPrecioDetalleDM a ListaPrecioDetalleDTO
            CreateMap<ListaPrecioDetalleDM, ListaPrecioDetalleDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.Ignore())
                .ForMember(dest => dest.Descripcion, opt => opt.Ignore());

            // Mapeo de ListaPrecioDetalleDTO a ListaPrecioDetalleDM
            CreateMap<ListaPrecioDetalleDTO, ListaPrecioDetalleDM>()
                .ConstructUsing(dto => new ListaPrecioDetalleDM(
                    dto.Id,
                    dto.IdListaPrecio,
                    dto.IdProducto,
                    dto.Precio
                ));
        }

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
