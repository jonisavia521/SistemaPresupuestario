using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando se violan reglas de validación del dominio
    /// DECISIÓN: Separar validaciones de dominio de validaciones técnicas para mejor trazabilidad
    /// </summary>
    public class DomainValidationException : Exception
    {
        /// <summary>
        /// Campo o propiedad que causó la validación
        /// </summary>
        public string Campo { get; }

        /// <summary>
        /// Valor que causó el error (para logging/debugging)
        /// </summary>
        public object ValorInvalido { get; }

        /// <summary>
        /// Constructor básico con mensaje
        /// </summary>
        /// <param name="mensaje">Descripción del error de validación</param>
        public DomainValidationException(string mensaje) : base(mensaje)
        {
        }

        /// <summary>
        /// Constructor con campo específico
        /// </summary>
        /// <param name="mensaje">Descripción del error</param>
        /// <param name="campo">Campo que causó la validación</param>
        public DomainValidationException(string mensaje, string campo) : base(mensaje)
        {
            Campo = campo;
        }

        /// <summary>
        /// Constructor completo con contexto detallado
        /// </summary>
        /// <param name="mensaje">Descripción del error</param>
        /// <param name="campo">Campo que causó la validación</param>
        /// <param name="valorInvalido">Valor que causó el error</param>
        public DomainValidationException(string mensaje, string campo, object valorInvalido) : base(mensaje)
        {
            Campo = campo;
            ValorInvalido = valorInvalido;
        }

        /// <summary>
        /// Constructor con excepción interna
        /// </summary>
        /// <param name="mensaje">Descripción del error</param>
        /// <param name="innerException">Excepción que causó este error</param>
        public DomainValidationException(string mensaje, Exception innerException) : base(mensaje, innerException)
        {
        }

        /// <summary>
        /// Constructor completo con excepción interna
        /// </summary>
        /// <param name="mensaje">Descripción del error</param>
        /// <param name="campo">Campo que causó la validación</param>
        /// <param name="valorInvalido">Valor que causó el error</param>
        /// <param name="innerException">Excepción que causó este error</param>
        public DomainValidationException(string mensaje, string campo, object valorInvalido, Exception innerException) 
            : base(mensaje, innerException)
        {
            Campo = campo;
            ValorInvalido = valorInvalido;
        }
    }
}