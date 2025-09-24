using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPresupuestario.BLL.Exceptions
{
    /// <summary>
    /// Exception thrown when domain validation rules are violated
    /// </summary>
    public class DomainValidationException : Exception
    {
        /// <summary>
        /// List of validation errors
        /// </summary>
        public List<ValidationError> ValidationErrors { get; }

        /// <summary>
        /// Property name that caused the validation error (if single property)
        /// </summary>
        public string PropertyName { get; }

        public DomainValidationException() : base("Domain validation failed")
        {
            ValidationErrors = new List<ValidationError>();
        }

        public DomainValidationException(string message) : base(message)
        {
            ValidationErrors = new List<ValidationError>();
        }

        public DomainValidationException(string message, Exception innerException) : base(message, innerException)
        {
            ValidationErrors = new List<ValidationError>();
        }

        public DomainValidationException(string propertyName, string errorMessage) : base(errorMessage)
        {
            PropertyName = propertyName;
            ValidationErrors = new List<ValidationError>
            {
                new ValidationError(propertyName, errorMessage)
            };
        }

        public DomainValidationException(IEnumerable<ValidationError> validationErrors) 
            : base(CreateErrorMessage(validationErrors))
        {
            ValidationErrors = validationErrors?.ToList() ?? new List<ValidationError>();
        }

        public DomainValidationException(string message, IEnumerable<ValidationError> validationErrors) 
            : base(message)
        {
            ValidationErrors = validationErrors?.ToList() ?? new List<ValidationError>();
        }

        /// <summary>
        /// Adds a validation error to the exception
        /// </summary>
        public void AddError(string propertyName, string errorMessage)
        {
            ValidationErrors.Add(new ValidationError(propertyName, errorMessage));
        }

        /// <summary>
        /// Checks if there are any validation errors
        /// </summary>
        public bool HasErrors => ValidationErrors.Any();

        /// <summary>
        /// Gets all error messages as a single string
        /// </summary>
        public string GetAllErrorMessages()
        {
            return string.Join("; ", ValidationErrors.Select(e => e.ErrorMessage));
        }

        /// <summary>
        /// Gets errors for a specific property
        /// </summary>
        public IEnumerable<string> GetErrorsForProperty(string propertyName)
        {
            return ValidationErrors
                .Where(e => string.Equals(e.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase))
                .Select(e => e.ErrorMessage);
        }

        private static string CreateErrorMessage(IEnumerable<ValidationError> errors)
        {
            if (errors == null || !errors.Any())
                return "Domain validation failed";

            var errorMessages = errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
            return $"Domain validation failed: {string.Join("; ", errorMessages)}";
        }
    }

    /// <summary>
    /// Represents a single validation error
    /// </summary>
    public class ValidationError
    {
        public string PropertyName { get; }
        public string ErrorMessage { get; }
        public object AttemptedValue { get; }

        public ValidationError(string propertyName, string errorMessage, object attemptedValue = null)
        {
            PropertyName = propertyName ?? string.Empty;
            ErrorMessage = errorMessage ?? string.Empty;
            AttemptedValue = attemptedValue;
        }

        public override string ToString()
        {
            return $"{PropertyName}: {ErrorMessage}";
        }
    }
}