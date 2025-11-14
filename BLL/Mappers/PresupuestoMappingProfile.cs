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
                // MODIFICADO: Usar totales persistidos en lugar de calcularlos
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal))
                .ForMember(dest => dest.TotalIva, opt => opt.MapFrom(src => src.TotalIva))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.ImporteArba, opt => opt.MapFrom(src => src.ImporteArba)) // NUEVO
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
                    dto.Detalles != null ? dto.Detalles.Select(d => MapToDetalleDomain(d)).ToList() : null,
                    dto.Subtotal,
                    dto.TotalIva,
                    dto.Total,
                    dto.ImporteArba // NUEVO
                ));

            // Mapeo de PresupuestoDetalleDM a PresupuestoDetalleDTO
            CreateMap<PresupuestoDetalleDM, PresupuestoDetalleDTO>()
                // MODIFICADO: Mapear TotalPersistido a Total
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.TotalPersistido))
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
                    dto.PorcentajeIVA,
                    dto.Total // Mapear Total persistido
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
            if (estado == 5) return "Facturado";
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
                dto.PorcentajeIVA,
                dto.Total // Mapear Total persistido
            );
        }
    }
}
