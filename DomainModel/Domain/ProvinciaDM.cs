using System;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio Provincia - Representa una provincia de Argentina
    /// Esta entidad es principalmente de referencia (catálogo)
    /// </summary>
    public class ProvinciaDM
    {
        public Guid Id { get; private set; }
        public string CodigoAFIP { get; private set; }
        public string Nombre { get; private set; }

        // Constructor para crear nueva provincia (para carga inicial)
        public ProvinciaDM(string codigoAFIP, string nombre)
        {
            Id = Guid.NewGuid();
            ValidarYEstablecerCodigoAFIP(codigoAFIP);
            ValidarYEstablecerNombre(nombre);
        }

        // Constructor para cargar desde base de datos
        public ProvinciaDM(Guid id, string codigoAFIP, string nombre)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID de la provincia no puede ser vacío.", nameof(id));

            Id = id;
            CodigoAFIP = codigoAFIP;
            Nombre = nombre;
        }

        // ==================== VALIDACIONES DE NEGOCIO ====================

        private void ValidarYEstablecerCodigoAFIP(string codigoAFIP)
        {
            if (string.IsNullOrWhiteSpace(codigoAFIP))
                throw new ArgumentException("El código AFIP es obligatorio.", nameof(codigoAFIP));

            if (codigoAFIP.Length != 2)
                throw new ArgumentException("El código AFIP debe tener exactamente 2 caracteres.", nameof(codigoAFIP));

            CodigoAFIP = codigoAFIP;
        }

        private void ValidarYEstablecerNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la provincia es obligatorio.", nameof(nombre));

            if (nombre.Length < 3)
                throw new ArgumentException("El nombre de la provincia debe tener al menos 3 caracteres.", nameof(nombre));

            if (nombre.Length > 50)
                throw new ArgumentException("El nombre de la provincia no puede exceder los 50 caracteres.", nameof(nombre));

            Nombre = nombre.Trim();
        }

        /// <summary>
        /// Método principal de validación de negocio
        /// </summary>
        public void ValidarNegocio()
        {
            ValidarYEstablecerCodigoAFIP(CodigoAFIP);
            ValidarYEstablecerNombre(Nombre);
        }
    }
}
