using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;
using System.Linq;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de Mapeo AutoMapper para Presupuesto y sus Detalles.
    /// 
    /// Define las transformaciones bidireccionales entre:
    /// - PresupuestoDM / PresupuestoDetalleDM (Modelos de Dominio): Contienen lógica de negocio
    /// - PresupuestoDTO / PresupuestoDetalleDTO (Data Transfer Objects): Para transferencia entre UI y BLL
    /// 
    /// Este perfil es más complejo que otros porque:
    /// 1. Maneja relaciones padre-hijo (Presupuesto ? Detalles)
    /// 2. Usa constructores personalizados de dominio (ConstructUsing)
    /// 3. Realiza transformaciones de enumeraciones de estados
    /// 4. Maneja colecciones anidadas
    /// </summary>
    public class PresupuestoMappingProfile : Profile
    {
        /// <summary>
        /// Constructor del perfil de mapeo de Presupuesto.
        /// Define todas las reglas de transformación.
        /// </summary>
        public PresupuestoMappingProfile()
        {
            /// <summary>
            /// Mapeo de PresupuestoDM a PresupuestoDTO.
            /// Se ejecuta cuando se retornan presupuestos desde BLL hacia la UI.
            /// 
            /// Totales: Se mapean directamente (Subtotal, TotalIva, Total, ImporteArba)
            /// Propiedades navegacionales: Se ignoran y se llenan por separado por el servicio
            /// Estado: Se transforma a descripción legible (1=Borrado, 2=Emitido, etc.)
            /// </summary>
            CreateMap<PresupuestoDM, PresupuestoDTO>()
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal))
                .ForMember(dest => dest.TotalIva, opt => opt.MapFrom(src => src.TotalIva))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.ImporteArba, opt => opt.MapFrom(src => src.ImporteArba))
                .ForMember(dest => dest.ClienteRazonSocial, opt => opt.Ignore())
                .ForMember(dest => dest.VendedorNombre, opt => opt.Ignore())
                .ForMember(dest => dest.EstadoDescripcion, opt => opt.MapFrom(src => ObtenerEstadoDescripcion(src.Estado)));

            /// <summary>
            /// Mapeo de PresupuestoDTO a PresupuestoDM.
            /// Se ejecuta cuando se envían presupuestos desde la UI hacia BLL.
            /// 
            /// Utiliza ConstructUsing para crear instancias usando el constructor de dominio
            /// que contiene validaciones y lógica de negocio.
            /// 
            /// Transforma la colección de detalles DTO a modelos de dominio usando
            /// el método auxiliar MapToDetalleDomain.
            /// </summary>
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
                    dto.ImporteArba
                ));

            /// <summary>
            /// Mapeo de PresupuestoDetalleDM a PresupuestoDetalleDTO.
            /// Se ejecuta cuando se retornan detalles de presupuestos desde BLL hacia la UI.
            /// 
            /// Total: Se mapea desde TotalPersistido (propiedad de BD)
            /// Código y Descripción: Se ignoran (se cargan por separado del producto)
            /// </summary>
            CreateMap<PresupuestoDetalleDM, PresupuestoDetalleDTO>()
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.TotalPersistido))
                .ForMember(dest => dest.Codigo, opt => opt.Ignore())
                .ForMember(dest => dest.Descripcion, opt => opt.Ignore());

            /// <summary>
            /// Mapeo de PresupuestoDetalleDTO a PresupuestoDetalleDM.
            /// Se ejecuta cuando se envían detalles desde la UI hacia BLL.
            /// 
            /// Utiliza ConstructUsing para crear instancias usando el constructor de dominio.
            /// </summary>
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
                    dto.Total
                ));
        }

        /// <summary>
        /// Obtiene la descripción legible de un código de estado de presupuesto.
        /// 
        /// Mapea los códigos numéricos de estado a sus descripciones:
        /// - 0 = "Borrador": Presupuesto en edición
        /// - 1 = "Emitido": Presupuesto creado y visible
        /// - 2 = "Aprobado": Cliente aceptó el presupuesto
        /// - 3 = "Rechazado": Cliente rechazó
        /// - 4 = "Vencido": Fecha de validez expirada
        /// - 5 = "Facturado": Convertido en factura
        /// - Otros = "Desconocido": Estado no reconocido
        /// 
        /// Se utiliza durante el mapeo PresupuestoDM ? PresupuestoDTO
        /// para proporcionar un EstadoDescripcion legible en la UI.
        /// </summary>
        /// <param name="estado">Código numérico del estado (0-6)</param>
        /// <returns>Descripción textual del estado</returns>
        private static string ObtenerEstadoDescripcion(int estado)
        {
            // Compatible con C# 7.3 - usa if-else en lugar de switch expressions
            if (estado == 0) return "Borrador";
            if (estado == 1) return "Emitido";
            if (estado == 2) return "Aprobado";
            if (estado == 3) return "Rechazado";
            if (estado == 4) return "Vencido";
            if (estado == 5) return "Facturado";
            return "Desconocido";
        }

        /// <summary>
        /// Método auxiliar para mapear un PresupuestoDetalleDTO a PresupuestoDetalleDM.
        /// 
        /// Se utiliza para transformar los detalles anidados dentro de un presupuesto.
        /// Crea una instancia de dominio usando el constructor especializado.
        /// </summary>
        /// <param name="dto">El detalle de presupuesto a transformar</param>
        /// <returns>Una instancia de PresupuestoDetalleDM con los datos del DTO</returns>
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
                dto.Total
            );
        }
    }
}
