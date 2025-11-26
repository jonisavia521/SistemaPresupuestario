using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) para Vendedor.
    /// 
    /// Esta clase actúa como intermediaria para transferir información de vendedores entre
    /// la capa de Presentación (UI) y la capa de Lógica de Negocio (BLL). Los DTOs
    /// desacoplan la estructura de datos de la base de datos de lo que se expone en la UI.
    /// 
    /// Un vendedor es una persona o entidad autorizada para crear cotizaciones y ventas
    /// en el sistema. Puede tener asignada una comisión sobre las operaciones que realiza.
    /// 
    /// Contiene atributos de validación DataAnnotations para realizar validaciones
    /// de entrada de datos en tiempo de enlace de modelos.
    /// </summary>
    public class VendedorDTO
    {
        /// <summary>
        /// Identificador único del vendedor (GUID).
        /// Se genera automáticamente al crear el vendedor y no es modificable.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Código interno asignado al vendedor para identificación rápida.
        /// Formato: Exactamente 2 dígitos (ej: "01", "02", "03").
        /// Debe ser único dentro del sistema.
        /// </summary>
        [Required(ErrorMessage = "El código de vendedor es obligatorio")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "El código debe tener exactamente 2 dígitos")]
        public string CodigoVendedor { get; set; }

        /// <summary>
        /// Nombre legal del vendedor.
        /// Se utiliza para identificación en documentos y reportes.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        public string Nombre { get; set; }

        /// <summary>
        /// Número de CUIT (Código Único de Identificación Tributaria) del vendedor.
        /// Formato: Exactamente 11 dígitos sin guiones ni espacios.
        /// Debe ser único dentro del sistema.
        /// Se utiliza para propósitos fiscales y legales.
        /// </summary>
        [Required(ErrorMessage = "El CUIT es obligatorio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El CUIT debe tener 11 dígitos")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "El CUIT solo puede contener dígitos")]
        public string CUIT { get; set; }

        /// <summary>
        /// Porcentaje de comisión asignado al vendedor.
        /// Se aplica al monto de ventas realizadas por este vendedor.
        /// Rango: 0 a 100 (representando 0% a 100%).
        /// Típicamente valores entre 5 y 20.
        /// </summary>
        [Required(ErrorMessage = "El porcentaje de comisión es obligatorio")]
        [Range(0, 100, ErrorMessage = "La comisión debe estar entre 0 y 100")]
        public decimal PorcentajeComision { get; set; }

        /// <summary>
        /// Dirección de correo electrónico del vendedor.
        /// Se utiliza para comunicaciones internas y envío de reportes.
        /// </summary>
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string Email { get; set; }

        /// <summary>
        /// Número de teléfono de contacto del vendedor.
        /// Se almacena como texto para permitir diferentes formatos regionales.
        /// </summary>
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string Telefono { get; set; }

        /// <summary>
        /// Dirección física del vendedor.
        /// </summary>
        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        public string Direccion { get; set; }

        /// <summary>
        /// Indicador del estado del vendedor.
        /// true = vendedor activo (puede crear cotizaciones y ventas)
        /// false = vendedor inactivo (desactivado lógicamente, se mantiene en histórico)
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el registro del vendedor.
        /// Se establece automáticamente al crear el vendedor y no es modificable.
        /// </summary>
        public DateTime FechaAlta { get; set; }

        /// <summary>
        /// Fecha y hora de la última modificación del vendedor.
        /// Se actualiza automáticamente cada vez que se modifica el registro.
        /// Nullable porque vendedores recién creados no tienen modificaciones.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Propiedad calculada que retorna el CUIT del vendedor en formato legible.
        /// Formato: "XX-XXXXXXXX-X" (ej: "23-12345678-9")
        /// Valida que el CUIT tenga exactamente 11 caracteres antes de formatear.
        /// Se utiliza en la UI para mostrar el CUIT de manera clara y legible.
        /// </summary>
        public string CUITFormateado
        {
            get
            {
                if (string.IsNullOrEmpty(CUIT) || CUIT.Length != 11)
                    return CUIT;
                return $"{CUIT.Substring(0, 2)}-{CUIT.Substring(2, 8)}-{CUIT.Substring(10, 1)}";
            }
        }

        /// <summary>
        /// Propiedad calculada que retorna el estado del vendedor en formato legible.
        /// Retorna "Activo" si Activo es true, "Inactivo" si es false.
        /// Se utiliza en listados de la UI para mostrar el estado de manera clara.
        /// </summary>
        public string EstadoTexto => Activo ? "Activo" : "Inactivo";
    }
}
