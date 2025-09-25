using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando se viola una regla de negocio del dominio
    /// </summary>
    public class DomainValidationException : Exception
    {
        /// <summary>
        /// Campo o propiedad que causó la validación
        /// </summary>
        public string Campo { get; }

        /// <summary>
        /// Código de error para identificar el tipo de validación
        /// </summary>
        public string CodigoError { get; }

        /// <summary>
        /// Constructor básico
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        public DomainValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor con campo específico
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="campo">Campo que causó la validación</param>
        public DomainValidationException(string message, string campo) : base(message)
        {
            Campo = campo;
        }

        /// <summary>
        /// Constructor completo
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="campo">Campo que causó la validación</param>
        /// <param name="codigoError">Código de error</param>
        public DomainValidationException(string message, string campo, string codigoError) : base(message)
        {
            Campo = campo;
            CodigoError = codigoError;
        }

        /// <summary>
        /// Constructor con inner exception
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="innerException">Excepción interna</param>
        public DomainValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor completo con inner exception
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="campo">Campo que causó la validación</param>
        /// <param name="codigoError">Código de error</param>
        /// <param name="innerException">Excepción interna</param>
        public DomainValidationException(string message, string campo, string codigoError, Exception innerException) 
            : base(message, innerException)
        {
            Campo = campo;
            CodigoError = codigoError;
        }
    }
}