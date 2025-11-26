using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) para Configuración del Sistema.
    /// 
    /// La configuración es un conjunto único de parámetros globales que definen
    /// los datos de la empresa y preferencias del sistema. Existe un solo registro
    /// de configuración por instancia de aplicación.
    /// 
    /// Incluye:
    /// - Información legal de la empresa (razón social, CUIT, tipo de IVA)
    /// - Datos de contacto (dirección, localidad, provincia, teléfono, email)
    /// - Preferencias del sistema (idioma)
    /// 
    /// Esta información se utiliza en:
    /// - Generación de documentos (facturas, cotizaciones)
    /// - Validaciones fiscales
    /// - Configuración del idioma de la interfaz
    /// </summary>
    public class ConfiguracionDTO
    {
        /// <summary>
        /// Identificador único de la configuración (GUID).
        /// Aunque teóricamente solo hay un registro, se mantiene el patrón de IDs
        /// para consistencia con el resto de DTOs.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Razón social (nombre legal) de la empresa.
        /// Este es el nombre oficial que aparecerá en todos los documentos fiscales
        /// (facturas, cotizaciones, etc.) emitidos por el sistema.
        /// </summary>
        [Required(ErrorMessage = "La razón social es obligatoria")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "La razón social debe tener entre 3 y 200 caracteres")]
        public string RazonSocial { get; set; }

        /// <summary>
        /// Número de CUIT (Código Único de Identificación Tributaria) de la empresa.
        /// Formato: Exactamente 11 dígitos sin guiones ni espacios.
        /// Se utiliza para identificación fiscal en documentos y comunicaciones con organismos.
        /// </summary>
        [Required(ErrorMessage = "El CUIT es obligatorio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El CUIT debe tener 11 dígitos")]
        public string CUIT { get; set; }

        /// <summary>
        /// Tipo de régimen de IVA de la empresa.
        /// Valores típicos: "RESPONSABLE_INSCRIPTO", "MONOTRIBUTISTA", "EXENTO", "NO_RESPONSABLE".
        /// Este valor afecta cálculos de impuestos y el formato de facturación.
        /// </summary>
        [Required(ErrorMessage = "El tipo de IVA es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo de IVA no puede exceder los 50 caracteres")]
        public string TipoIva { get; set; }

        /// <summary>
        /// Dirección física de la empresa.
        /// Se utiliza en documentos fiscales y como dirección de contacto principal.
        /// Formato típico: "Calle, Número, Piso, Departamento"
        /// </summary>
        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        public string Direccion { get; set; }

        /// <summary>
        /// Localidad o ciudad donde se ubica la empresa.
        /// </summary>
        [StringLength(100, ErrorMessage = "La localidad no puede exceder los 100 caracteres")]
        public string Localidad { get; set; }

        /// <summary>
        /// Identificador (GUID) de la provincia donde se domicilia la empresa.
        /// Esta es una clave foránea a la entidad Provincia.
        /// Nullable porque la provincia podría no estar registrada en el sistema.
        /// </summary>
        public Guid? IdProvincia { get; set; }

        /// <summary>
        /// Dirección de correo electrónico de contacto de la empresa.
        /// Se utiliza como dirección de respuesta en comunicaciones del sistema
        /// y como contacto para envío de reportes.
        /// </summary>
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string Email { get; set; }

        /// <summary>
        /// Número de teléfono principal de la empresa.
        /// Se almacena como texto para permitir diferentes formatos regionales
        /// (ej: "011 1234-5678", "+54 9 11 1234-5678").
        /// </summary>
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string Telefono { get; set; }

        /// <summary>
        /// Idioma configurado para la interfaz de usuario.
        /// Ejemplos: "es-AR" (Español - Argentina), "en-US" (Inglés - Estados Unidos).
        /// Este parámetro se carga al iniciar la aplicación para traducir la UI.
        /// </summary>
        [Required(ErrorMessage = "El idioma es obligatorio")]
        public string Idioma { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el registro de configuración.
        /// Se establece automáticamente en la primera creación y no es modificable.
        /// </summary>
        public DateTime FechaAlta { get; set; }

        /// <summary>
        /// Fecha y hora de la última modificación de la configuración.
        /// Se actualiza automáticamente cada vez que se modifica la configuración del sistema.
        /// Nullable porque la configuración recién creada no tiene modificaciones.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }
    }
}
