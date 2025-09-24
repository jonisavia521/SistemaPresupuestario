using System;

namespace SistemaPresupuestario.BLL.Exceptions
{
    /// <summary>
    /// Exception thrown when business rules are violated
    /// </summary>
    public class BusinessRuleException : Exception
    {
        /// <summary>
        /// The business rule that was violated
        /// </summary>
        public string RuleName { get; }

        /// <summary>
        /// Additional context about the rule violation
        /// </summary>
        public object Context { get; }

        /// <summary>
        /// Severity level of the business rule violation
        /// </summary>
        public BusinessRuleSeverity Severity { get; }

        public BusinessRuleException() : base("A business rule was violated")
        {
            Severity = BusinessRuleSeverity.Error;
        }

        public BusinessRuleException(string message) : base(message)
        {
            Severity = BusinessRuleSeverity.Error;
        }

        public BusinessRuleException(string message, Exception innerException) : base(message, innerException)
        {
            Severity = BusinessRuleSeverity.Error;
        }

        public BusinessRuleException(string ruleName, string message) : base(message)
        {
            RuleName = ruleName;
            Severity = BusinessRuleSeverity.Error;
        }

        public BusinessRuleException(string ruleName, string message, BusinessRuleSeverity severity) : base(message)
        {
            RuleName = ruleName;
            Severity = severity;
        }

        public BusinessRuleException(string ruleName, string message, object context) : base(message)
        {
            RuleName = ruleName;
            Context = context;
            Severity = BusinessRuleSeverity.Error;
        }

        public BusinessRuleException(string ruleName, string message, object context, BusinessRuleSeverity severity) : base(message)
        {
            RuleName = ruleName;
            Context = context;
            Severity = severity;
        }

        /// <summary>
        /// Gets a user-friendly message for the business rule violation
        /// </summary>
        public string GetUserFriendlyMessage()
        {
            return RuleName switch
            {
                "UserMustHavePermissions" => "El usuario debe tener al menos una familia o patente asignada.",
                "UsernameAlreadyExists" => "Ya existe un usuario con ese nombre de usuario.",
                "FamilyCycleDetected" => "No se puede crear la relación porque generaría un ciclo en la jerarquía de familias.",
                "CannotDeleteUserWithActiveSession" => "No se puede eliminar un usuario que tiene una sesión activa.",
                "PasswordTooWeak" => "La contraseña no cumple con los requisitos mínimos de seguridad.",
                "PatentNameAlreadyExists" => "Ya existe una patente con ese nombre.",
                "FamilyNameAlreadyExists" => "Ya existe una familia con ese nombre.",
                "CannotDeleteFamilyWithUsers" => "No se puede eliminar una familia que tiene usuarios asignados.",
                "CannotDeleteFamilyWithChildren" => "No se puede eliminar una familia que tiene familias hijas.",
                "CannotDeletePatentInUse" => "No se puede eliminar una patente que está siendo utilizada.",
                _ => Message
            };
        }

        /// <summary>
        /// Creates a business rule exception for username already exists
        /// </summary>
        public static BusinessRuleException UsernameAlreadyExists(string username)
        {
            return new BusinessRuleException("UsernameAlreadyExists", 
                $"Ya existe un usuario con el nombre de usuario '{username}'.", username);
        }

        /// <summary>
        /// Creates a business rule exception for user without permissions
        /// </summary>
        public static BusinessRuleException UserMustHavePermissions()
        {
            return new BusinessRuleException("UserMustHavePermissions", 
                "El usuario debe tener al menos una familia o patente asignada.");
        }

        /// <summary>
        /// Creates a business rule exception for family cycle detection
        /// </summary>
        public static BusinessRuleException FamilyCycleDetected(string childName, string parentName)
        {
            return new BusinessRuleException("FamilyCycleDetected", 
                $"No se puede asignar '{parentName}' como padre de '{childName}' porque generaría un ciclo en la jerarquía.",
                new { ChildName = childName, ParentName = parentName });
        }

        /// <summary>
        /// Creates a business rule exception for password requirements
        /// </summary>
        public static BusinessRuleException PasswordTooWeak(string requirements)
        {
            return new BusinessRuleException("PasswordTooWeak", 
                $"La contraseña no cumple con los requisitos: {requirements}");
        }
    }

    /// <summary>
    /// Severity levels for business rule violations
    /// </summary>
    public enum BusinessRuleSeverity
    {
        /// <summary>
        /// Information level - not blocking
        /// </summary>
        Info,

        /// <summary>
        /// Warning level - may proceed with caution
        /// </summary>
        Warning,

        /// <summary>
        /// Error level - operation should not proceed
        /// </summary>
        Error,

        /// <summary>
        /// Critical level - system integrity at risk
        /// </summary>
        Critical
    }
}