using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) para Cliente.
    /// 
    /// Esta clase actúa como intermediaria para transferir información de clientes entre
    /// la capa de Presentación (UI) y la capa de Lógica de Negocio (BLL). Los DTOs
    /// desacoplan la estructura de datos de la base de datos de lo que se expone en la UI.
    /// 
    /// Contiene atributos de validación DataAnnotations para realizar validaciones
    /// de entrada de datos en tiempo de enlace de modelos.
    /// 
    /// Propiedades principales:
    /// - Identificación: CodigoCliente, NumeroDocumento
    /// - Información de contacto: Email, Telefono, Direccion, Localidad
    /// - Referencias a entidades relacionadas: IdVendedor, IdProvincia
    /// - Información fiscal: TipoIva, AlicuotaArba
    /// - Control: Activo, FechaAlta, FechaModificacion
    /// </summary>
    public class ClienteDTO
    {
        /// <summary>
        /// Identificador único del cliente (GUID).
        /// Se genera automáticamente al crear el cliente y no es modificable.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Código interno asignado al cliente para identificación rápida en la UI.
        /// Debe ser único dentro del sistema.
        /// </summary>
        [Required(ErrorMessage = "El código de cliente es obligatorio")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 20 caracteres")]
        public string CodigoCliente { get; set; }

        /// <summary>
        /// Razón social o nombre legal de la empresa cliente.
        /// Este es el nombre oficial que aparecerá en documentos fiscales.
        /// </summary>
        [Required(ErrorMessage = "La razón social es obligatoria")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "La razón social debe tener entre 3 y 200 caracteres")]
        public string RazonSocial { get; set; }

        /// <summary>
        /// Tipo de documento fiscal del cliente (ej: CUIT, DNI, etc.).
        /// Define qué tipo de identificación fiscal utiliza la empresa.
        /// </summary>
        [Required(ErrorMessage = "El tipo de documento es obligatorio")]
        public string TipoDocumento { get; set; }

        /// <summary>
        /// Número de documento fiscal del cliente (CUIT, DNI, etc.).
        /// Debe ser único y debe validarse su formato según el tipo de documento.
        /// </summary>
        [Required(ErrorMessage = "El número de documento es obligatorio")]
        public string NumeroDocumento { get; set; }

        /// <summary>
        /// Identificador (GUID) del vendedor asignado a este cliente.
        /// Nullable porque un cliente puede no tener vendedor asignado.
        /// Esta es una clave foránea a la entidad Vendedor.
        /// </summary>
        public Guid? IdVendedor { get; set; }

        /// <summary>
        /// Propiedad auxiliar que contiene el código del vendedor asignado.
        /// Se utiliza en la UI para mostrar el vendedor sin requererir una carga adicional.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string CodigoVendedor { get; set; }
        
        /// <summary>
        /// Propiedad auxiliar que contiene el nombre del vendedor asignado.
        /// Se utiliza en la UI para mostrar el nombre del vendedor.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string NombreVendedor { get; set; }

        /// <summary>
        /// Identificador (GUID) de la provincia donde se domicilia el cliente.
        /// Nullable porque un cliente puede no tener provincia asignada.
        /// Esta es una clave foránea a la entidad Provincia.
        /// </summary>
        public Guid? IdProvincia { get; set; }

        /// <summary>
        /// Propiedad auxiliar que contiene el nombre de la provincia del cliente.
        /// Se utiliza en la UI para mostrar la provincia sin requerer una carga adicional.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string NombreProvincia { get; set; }

        /// <summary>
        /// Tipo de régimen de IVA del cliente para propósitos fiscales.
        /// Valores típicos: "RESPONSABLE_INSCRIPTO", "MONOTRIBUTISTA", "EXENTO", "CONSUMIDOR_FINAL", "NO_RESPONSABLE".
        /// Este valor afecta cálculos de impuestos y percepciones ARBA.
        /// </summary>
        [Required(ErrorMessage = "El tipo de IVA es obligatorio")]
        public string TipoIva { get; set; }

        /// <summary>
        /// Condición de pago acordada con el cliente.
        /// Formato: código de 2 dígitos (ej: "01" para contado, "02" para 30 días).
        /// Define los términos de pago por defecto para cotizaciones del cliente.
        /// </summary>
        [Required(ErrorMessage = "La condición de pago es obligatoria")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "La condición de pago debe tener 2 dígitos")]
        public string CondicionPago { get; set; }

        /// <summary>
        /// Alícuota ARBA (Agencia de Recaudación de Buenos Aires) aplicable al cliente.
        /// Se utiliza para calcular percepciones en operaciones donde aplica la jurisdicción ARBA.
        /// Rango típico: 0.5% a 5% para clientes "Responsables Inscriptos".
        /// </summary>
        [Required(ErrorMessage = "La alícuota ARBA es obligatoria")]
        [Range(0, 100, ErrorMessage = "La alícuota ARBA debe estar entre 0 y 100")]
        public decimal AlicuotaArba { get; set; }

        /// <summary>
        /// Dirección de correo electrónico del cliente.
        /// Se utiliza para enviar cotizaciones y facturas por correo.
        /// </summary>
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string Email { get; set; }

        /// <summary>
        /// Número de teléfono de contacto del cliente.
        /// Se almacena como texto para permitir diferentes formatos regionales.
        /// </summary>
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string Telefono { get; set; }

        /// <summary>
        /// Dirección física del cliente (calle, número, piso, depto).
        /// Se utiliza para entregas y documentos fiscales.
        /// </summary>
        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        public string Direccion { get; set; }

        /// <summary>
        /// Localidad o ciudad donde se ubica el cliente.
        /// </summary>
        [StringLength(100, ErrorMessage = "La localidad no puede exceder los 100 caracteres")]
        public string Localidad { get; set; }

        /// <summary>
        /// Indicador del estado del cliente.
        /// true = cliente activo (puede participar en operaciones)
        /// false = cliente inactivo (desactivado lógicamente, se mantiene en histórico)
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el registro del cliente.
        /// Se establece automáticamente al crear el cliente y no es modificable.
        /// </summary>
        public DateTime FechaAlta { get; set; }

        /// <summary>
        /// Fecha y hora de la última modificación del cliente.
        /// Se actualiza automáticamente cada vez que se modifica el registro.
        /// Nullable porque clientes recién creados no tienen modificaciones.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Propiedad calculada que retorna una representación legible del documento del cliente.
        /// Formato: "{TipoDocumento}: {NumeroDocumento}" (ej: "CUIT: 12345678901")
        /// Se utiliza en la UI para mostrar el documento de forma clara.
        /// </summary>
        public string DocumentoCompleto => $"{TipoDocumento}: {NumeroDocumento}";

        /// <summary>
        /// Propiedad calculada que retorna el estado del cliente en formato legible.
        /// Retorna "Activo" si Activo es true, "Inactivo" si es false.
        /// Se utiliza en listados de la UI para mostrar el estado de manera clara.
        /// </summary>
        public string EstadoTexto => Activo ? "Activo" : "Inactivo";
    }
}
