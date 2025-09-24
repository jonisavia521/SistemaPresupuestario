using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// Excepción para conflictos de concurrencia
    /// </summary>
    public class ConcurrencyException : Exception
    {
        public string EntityName { get; }
        public object EntityId { get; }

        public ConcurrencyException(string message) : base(message)
        {
        }

        public ConcurrencyException(string entityName, object entityId, string message) 
            : base(message)
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}