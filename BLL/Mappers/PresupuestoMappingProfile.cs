using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;
using System.Linq;

namespace BLL.Mappers
{
    public class PresupuestoMappingProfile : Profile
    {
        public PresupuestoMappingProfile()
        {
            // Mapeo de PresupuestoDM a PresupuestoDTO
            CreateMap<PresupuestoDM, PresupuestoDTO>()
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.CalcularSubtotal()))
                .ForMember(dest => dest.TotalIva, opt => opt.MapFrom(src => src.CalcularIva()))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.CalcularTotal()))
                .ForMember(dest => dest.ClienteRazonSocial, opt => opt.Ignore())
                .ForMember(dest => dest.VendedorNombre, opt => opt.Ignore())
                .ForMember(dest => dest.EstadoDescripcion, opt => opt.MapFrom(src => ObtenerEstadoDescripcion(src.Estado)));

            // Mapeo de PresupuestoDTO a PresupuestoDM
            CreateMap<PresupuestoDTO, PresupuestoDM>()
                .ConstructUsing(dto => new PresupuestoDM(
                    dto.Id,
                    dto.Numero,
                    dto.IdCliente,
                    dto.FechaEmision,
                    dto.Estado,
                    dto.FechaVencimiento,
                    dto.IdPresupuestoPadre,
                    dto.IdVendedor,
                    dto.Detalles != null ? dto.Detalles.Select(d => MapToDetalleDomain(d)).ToList() : null
                ));

            // Mapeo de PresupuestoDetalleDM a PresupuestoDetalleDTO
            CreateMap<PresupuestoDetalleDM, PresupuestoDetalleDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.Ignore())
                .ForMember(dest => dest.Descripcion, opt => opt.Ignore());

            // Mapeo de PresupuestoDetalleDTO a PresupuestoDetalleDM
            CreateMap<PresupuestoDetalleDTO, PresupuestoDetalleDM>()
                .ConstructUsing(dto => new PresupuestoDetalleDM(
                    dto.Id,
                    dto.Numero,
                    dto.IdPresupuesto,
                    dto.IdProducto,
                    dto.Cantidad,
                    dto.Precio,
                    dto.Descuento,
                    dto.Renglon,
                    dto.PorcentajeIVA
                ));
        }

        private static string ObtenerEstadoDescripcion(int estado)
        {
            // Compatible con C# 7.3 - reemplazar switch expression por if-else
            if (estado == 0) return "Borrador";
            if (estado == 1) return "Emitido";
            if (estado == 2) return "Aprobado";
            if (estado == 3) return "Rechazado";
            if (estado == 4) return "Vencido";
            return "Desconocido";
        }

        private static PresupuestoDetalleDM MapToDetalleDomain(PresupuestoDetalleDTO dto)
        {
            return new PresupuestoDetalleDM(
                dto.Id,
                dto.Numero,
                dto.IdPresupuesto,
                dto.IdProducto,
                dto.Cantidad,
                dto.Precio,
                dto.Descuento,
                dto.Renglon,
                dto.PorcentajeIVA
            );
        }
    }
}
