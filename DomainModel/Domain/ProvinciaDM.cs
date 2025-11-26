using System;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio que representa una provincia de Argentina.
    /// 
    /// Esta es principalmente una entidad de referencia o catálogo, utilizada para
    /// seleccionar la ubicación de clientes y empresa. Contiene los códigos AFIP
    /// oficiales para todas las provincias argentinas.
    /// 
    /// Responsabilidades:
    /// - Mantener catálogo de provincias
    /// - Validar códigos AFIP
    /// - Validar nombres de provincias
    /// 
    /// Invariantes:
    /// - Código AFIP exactamente 2 caracteres
    /// - Nombre entre 3 y 50 caracteres
    /// 
    /// Notas:
    /// - Los datos de provincias se cargan en la inicialización de la BD
    /// - Generalmente no se crean nuevas provincias después de eso
    /// </summary>
    public class ProvinciaDM
    {
        /// <summary>Identificador único de la provincia en el sistema</summary>
        public Guid Id { get; private set; }
        
        /// <summary>Código AFIP de la provincia. Exactamente 2 caracteres.</summary>
        public string CodigoAFIP { get; private set; }
        
        /// <summary>Nombre completo de la provincia. Entre 3 y 50 caracteres.</summary>
        public string Nombre { get; private set; }

        /// <summary>
        /// Crea una nueva instancia de provincia para carga inicial del catálogo.
        /// 
        /// Se validan automáticamente los parámetros.
        /// </summary>
        /// <param name="codigoAFIP">Código AFIP de exactamente 2 caracteres</param>
        /// <param name="nombre">Nombre de la provincia (3-50 caracteres)</param>
        /// <exception cref="ArgumentException">Si los parámetros no cumplen validaciones</exception>
        /// <exception cref="ArgumentNullException">Si un parámetro requerido es nulo</exception>
        public ProvinciaDM(string codigoAFIP, string nombre)
        {
            Id = Guid.NewGuid();
            ValidarYEstablecerCodigoAFIP(codigoAFIP);
            ValidarYEstablecerNombre(nombre);
        }

        /// <summary>
        /// Constructor para hidratar una provincia desde la base de datos.
        /// </summary>
        /// <param name="id">GUID único de la provincia</param>
        /// <param name="codigoAFIP">Código AFIP</param>
        /// <param name="nombre">Nombre de la provincia</param>
        /// <exception cref="ArgumentException">Si el ID es un GUID vacío</exception>
        public ProvinciaDM(Guid id, string codigoAFIP, string nombre)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID de la provincia no puede ser vacío.", nameof(id));

            Id = id;
            CodigoAFIP = codigoAFIP;
            Nombre = nombre;
        }

        // Validaciones de negocio
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
        /// Valida todos los datos de la provincia contra las reglas de negocio.
        /// </summary>
        /// <exception cref="ArgumentException">Si alguna propiedad no cumple validaciones</exception>
        public void ValidarNegocio()
        {
            ValidarYEstablecerCodigoAFIP(CodigoAFIP);
            ValidarYEstablecerNombre(Nombre);
        }
    }
}
