using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// Excepción para errores de validación de dominio
    /// </summary>
    public class DomainValidationException : Exception
    {
        public string PropertyName { get; }
        public object AttemptedValue { get; }

        public DomainValidationException(string message) : base(message)
        {
        }

        public DomainValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DomainValidationException(string propertyName, object attemptedValue, string message) 
            : base(message)
        {
            PropertyName = propertyName;
            AttemptedValue = attemptedValue;
        }
    }
}