using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) para Producto.
    /// 
    /// Esta clase actúa como intermediaria para transferir información de productos o servicios
    /// entre la capa de Presentación (UI) y la capa de Lógica de Negocio (BLL).
    /// 
    /// Un producto es un artículo que la empresa ofrece a sus clientes. Puede ser un bien
    /// físico o un servicio. Los productos se utilizan para construir las líneas de cotizaciones
    /// y facturas. Incluye información del producto y su clasificación fiscal (IVA).
    /// 
    /// Contiene atributos de validación DataAnnotations para realizar validaciones
    /// de entrada de datos en tiempo de enlace de modelos.
    /// </summary>
    public class ProductoDTO
    {
        /// <summary>
        /// Identificador único del producto (GUID).
        /// Se genera automáticamente al crear el producto y no es modificable.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Código interno asignado al producto para identificación rápida.
        /// Debe ser único dentro del sistema.
        /// Se utiliza como referencia en cotizaciones, facturas e inventario.
        /// </summary>
        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(50, ErrorMessage = "El código no puede exceder los 50 caracteres")]
        public string Codigo { get; set; }

        /// <summary>
        /// Descripción textual del producto.
        /// Proporciona información adicional sobre el artículo para su identificación.
        /// Se incluye en cotizaciones y facturas para claridad del cliente.
        /// </summary>
        [StringLength(50, ErrorMessage = "La descripción no puede exceder los 50 caracteres")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Indicador de si el producto está inhabilitado.
        /// true = producto inhabilitado (no disponible para nuevas cotizaciones)
        /// false = producto activo (puede utilizarse en cotizaciones)
        /// Aunque inhabilitado, se mantiene en histórico para productos ya vendidos.
        /// </summary>
        public bool Inhabilitado { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el registro del producto.
        /// Se establece automáticamente al crear el producto y no es modificable.
        /// </summary>
        public DateTime FechaAlta { get; set; }

        /// <summary>
        /// Identificador del usuario que creó el registro del producto.
        /// Se establece automáticamente y se utiliza para auditoría.
        /// </summary>
        public int UsuarioAlta { get; set; }

        /// <summary>
        /// Porcentaje de Impuesto al Valor Agregado (IVA) aplicable al producto.
        /// Valores típicos: 0 (Exento), 10.5 (IVA Reducido), 21 (IVA General).
        /// Se utiliza para cálculos de montos en cotizaciones y facturas.
        /// Rango permitido: 0 a 21.
        /// </summary>
        [Required(ErrorMessage = "El porcentaje de IVA es obligatorio")]
        [Range(0, 21, ErrorMessage = "El porcentaje de IVA debe estar entre 0 y 21")]
        public decimal PorcentajeIVA { get; set; }

        /// <summary>
        /// Propiedad calculada que retorna el estado del producto en formato legible.
        /// Retorna "Inactivo" si Inhabilitado es true, "Activo" si es false.
        /// Se utiliza en listados de la UI para mostrar el estado de manera clara.
        /// </summary>
        public string EstadoTexto => Inhabilitado ? "Inactivo" : "Activo";

        /// <summary>
        /// Propiedad calculada que retorna el porcentaje de IVA en formato legible.
        /// Interpreta valores especiales:
        /// - 0 como "Exento"
        /// - 10.5 como "10.5%"
        /// - 21 como "21%"
        /// - Otros valores como "{porcentaje}%"
        /// Se utiliza en la UI para mostrar el IVA de manera clara y uniforme.
        /// </summary>
        public string IVATexto
        {
            get
            {
                if (PorcentajeIVA == 0)
                    return "Exento";
                else if (PorcentajeIVA == 10.50m)
                    return "10.5%";
                else if (PorcentajeIVA == 21)
                    return "21%";
                else
                    return $"{PorcentajeIVA}%";
            }
        }
    }
}
