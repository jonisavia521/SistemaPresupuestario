using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) para Provincia.
    /// 
    /// Una provincia es una división administrativa de Argentina. Este DTO
    /// transfiere información sobre provincias entre la capa de Presentación (UI)
    /// y la capa de Lógica de Negocio (BLL).
    /// 
    /// Las provincias se utilizan principalmente en dos contextos:
    /// 1. Para completar información de dirección de clientes
    /// 2. Para cálculos de impuestos jurisdiccionales (especialmente ARBA para Buenos Aires)
    /// 
    /// Los códigos AFIP (Administración Federal de Ingresos Públicos) son estándares
    /// en la documentación fiscal argentina y sistemas integrados.
    /// </summary>
    public class ProvinciaDTO
    {
        /// <summary>
        /// Identificador único de la provincia (GUID).
        /// Se genera automáticamente al crear el registro y no es modificable.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Código de identificación AFIP (Administración Federal de Ingresos Públicos).
        /// Formato: Exactamente 2 caracteres (dígitos o letras).
        /// Ejemplos:
        /// - "01" = Buenos Aires
        /// - "02" = Catamarca
        /// - "05" = Corrientes
        /// - "99" = Exterior
        /// 
        /// Este código es estándar a nivel nacional y utilizado en documentación fiscal.
        /// Debe ser único dentro del sistema.
        /// </summary>
        [Required(ErrorMessage = "El código AFIP es obligatorio")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "El código AFIP debe tener exactamente 2 caracteres")]
        public string CodigoAFIP { get; set; }

        /// <summary>
        /// Nombre oficial de la provincia.
        /// Ejemplos: "Buenos Aires", "Córdoba", "Santa Fe", "Mendoza".
        /// Se utiliza en formularios, listas desplegables y reportes para que el usuario
        /// identifique la provincia de forma clara.
        /// </summary>
        [Required(ErrorMessage = "El nombre de la provincia es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        public string Nombre { get; set; }

        /// <summary>
        /// Propiedad calculada que retorna el nombre completo en formato "CÓDIGO - NOMBRE".
        /// Ejemplo: "01 - Buenos Aires"
        /// Se utiliza en listas desplegables (ComboBox) para mostrar tanto el código como el nombre
        /// de forma clara y permitir que el usuario identifique la provincia rápidamente.
        /// </summary>
        public string NombreCompleto => $"{CodigoAFIP} - {Nombre}";
    }
}
