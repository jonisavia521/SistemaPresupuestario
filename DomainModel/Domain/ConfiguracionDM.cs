using System;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio que representa la configuración general del sistema.
    /// 
    /// Esta tabla siempre contiene un único registro con los datos de la empresa
    /// utilizadora del sistema de presupuestos: razón social, CUIT, tipo de IVA,
    /// dirección, idioma, etc. 
    /// 
    /// Responsabilidades:
    /// - Validar datos fiscales de la empresa
    /// - Gestionar idioma del sistema
    /// - Validar CUIT con algoritmo AFIP
    /// - Mantener datos de contacto de la empresa
    /// 
    /// Invariantes:
    /// - Existe un único registro en la tabla
    /// - Razón social obligatoria entre 3 y 200 caracteres
    /// - CUIT válido de 11 dígitos con verificador
    /// - Tipo de IVA en categorías AFIP permitidas
    /// - Idioma soportado: es-AR o en-US
    /// 
    /// </summary>
    public class ConfiguracionDM
    {
        /// <summary>Identificador único de la configuración (siempre uno)</summary>
        public Guid Id { get; private set; }
        
        /// <summary>Razón social de la empresa. Entre 3 y 200 caracteres.</summary>
        public string RazonSocial { get; private set; }
        
        /// <summary>Número CUIT de la empresa (11 dígitos validados)</summary>
        public string CUIT { get; private set; }
        
        /// <summary>Categoría de IVA de la empresa según AFIP</summary>
        public string TipoIva { get; private set; }
        
        /// <summary>Dirección postal de la empresa (opcional)</summary>
        public string Direccion { get; private set; }
        
        /// <summary>Localidad o ciudad de la empresa (opcional)</summary>
        public string Localidad { get; private set; }
        
        /// <summary>Identificador de la provincia donde está domiciliada la empresa</summary>
        public Guid? IdProvincia { get; private set; }
        
        /// <summary>Correo electrónico de contacto de la empresa (opcional)</summary>
        public string Email { get; private set; }
        
        /// <summary>Número de teléfono principal de la empresa (opcional)</summary>
        public string Telefono { get; private set; }
        
        /// <summary>Idioma del sistema: es-AR (Español Argentina) o en-US (Inglés USA). Predeterminado: es-AR</summary>
        public string Idioma { get; private set; }
        
        /// <summary>Fecha y hora en que se creó el registro de configuración</summary>
        public DateTime FechaAlta { get; private set; }
        
        /// <summary>Fecha y hora de la última modificación de la configuración</summary>
        public DateTime? FechaModificacion { get; private set; }

        /// <summary>
        /// Crea una nueva instancia de configuración del sistema.
        /// 
        /// Se validan automáticamente todos los parámetros. Típicamente se crea
        /// una sola instancia de esta clase al inicializar el sistema.
        /// </summary>
        /// <param name="razonSocial">Razón social de la empresa (3-200 caracteres)</param>
        /// <param name="cuit">CUIT de la empresa (11 dígitos con verificador)</param>
        /// <param name="tipoIva">Categoría de IVA de la empresa</param>
        /// <param name="idioma">Idioma del sistema (es-AR o en-US, predeterminado es-AR)</param>
        /// <param name="direccion">Dirección postal (opcional)</param>
        /// <param name="localidad">Localidad o ciudad (opcional)</param>
        /// <param name="idProvincia">GUID de la provincia (opcional)</param>
        /// <param name="email">Correo electrónico (opcional)</param>
        /// <param name="telefono">Teléfono (opcional)</param>
        /// <exception cref="ArgumentException">Si algún parámetro no cumple validaciones</exception>
        /// <exception cref="ArgumentNullException">Si un parámetro requerido es nulo</exception>
        public ConfiguracionDM(
            string razonSocial,
            string cuit,
            string tipoIva,
            string idioma = "es-AR",
            string direccion = null,
            string localidad = null,
            Guid? idProvincia = null,
            string email = null,
            string telefono = null)
        {
            Id = Guid.NewGuid();
            FechaAlta = DateTime.Now;

            ValidarYEstablecerRazonSocial(razonSocial);
            ValidarYEstablecerCUIT(cuit);
            ValidarYEstablecerTipoIva(tipoIva);
            ValidarYEstablecerIdioma(idioma);
            
            Direccion = direccion;
            Localidad = localidad;
            IdProvincia = idProvincia;
            Email = email;
            Telefono = telefono;
        }

        /// <summary>
        /// Constructor para hidratar la configuración desde la base de datos.
        /// 
        /// Se utiliza internamente por los repositorios.
        /// </summary>
        /// <param name="id">GUID único de la configuración</param>
        /// <param name="razonSocial">Razón social de la empresa</param>
        /// <param name="cuit">CUIT de la empresa</param>
        /// <param name="tipoIva">Categoría de IVA</param>
        /// <param name="idioma">Idioma del sistema</param>
        /// <param name="fechaAlta">Fecha de creación</param>
        /// <param name="fechaModificacion">Fecha de última modificación (puede ser nula)</param>
        /// <param name="direccion">Dirección (puede ser nula)</param>
        /// <param name="localidad">Localidad (puede ser nula)</param>
        /// <param name="idProvincia">GUID de provincia (puede ser nulo)</param>
        /// <param name="email">Correo (puede ser nulo)</param>
        /// <param name="telefono">Teléfono (puede ser nulo)</param>
        /// <exception cref="ArgumentException">Si el ID es un GUID vacío</exception>
        public ConfiguracionDM(
            Guid id,
            string razonSocial,
            string cuit,
            string tipoIva,
            string idioma,
            DateTime fechaAlta,
            DateTime? fechaModificacion = null,
            string direccion = null,
            string localidad = null,
            Guid? idProvincia = null,
            string email = null,
            string telefono = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID de configuración no puede ser vacío.", nameof(id));

            Id = id;
            RazonSocial = razonSocial;
            CUIT = cuit;
            TipoIva = tipoIva;
            Idioma = idioma;
            FechaAlta = fechaAlta;
            FechaModificacion = fechaModificacion;
            Direccion = direccion;
            Localidad = localidad;
            IdProvincia = idProvincia;
            Email = email;
            Telefono = telefono;
        }

        /// <summary>
        /// Actualiza todos los datos de la configuración del sistema.
        /// 
        /// Valida todos los parámetros según las reglas de negocio y actualiza
        /// la fecha de modificación. Se utiliza cuando el usuario modifica la
        /// configuración desde la interfaz de administración.
        /// </summary>
        /// <param name="razonSocial">Nueva razón social de la empresa</param>
        /// <param name="cuit">Nuevo CUIT de la empresa</param>
        /// <param name="tipoIva">Nueva categoría de IVA</param>
        /// <param name="idioma">Nuevo idioma del sistema</param>
        /// <param name="direccion">Nueva dirección (puede ser nula)</param>
        /// <param name="localidad">Nueva localidad (puede ser nula)</param>
        /// <param name="idProvincia">Nueva provincia (puede ser nula)</param>
        /// <param name="email">Nuevo correo (puede ser nulo)</param>
        /// <param name="telefono">Nuevo teléfono (puede ser nulo)</param>
        /// <exception cref="ArgumentException">Si algún parámetro no cumple validaciones</exception>
        public void ActualizarDatos(
            string razonSocial,
            string cuit,
            string tipoIva,
            string idioma,
            string direccion = null,
            string localidad = null,
            Guid? idProvincia = null,
            string email = null,
            string telefono = null)
        {
            ValidarYEstablecerRazonSocial(razonSocial);
            ValidarYEstablecerCUIT(cuit);
            ValidarYEstablecerTipoIva(tipoIva);
            ValidarYEstablecerIdioma(idioma);
            
            Direccion = direccion;
            Localidad = localidad;
            IdProvincia = idProvincia;
            Email = email;
            Telefono = telefono;
            FechaModificacion = DateTime.Now;
        }

        // Validaciones de negocio
        private void ValidarYEstablecerRazonSocial(string razonSocial)
        {
            if (string.IsNullOrWhiteSpace(razonSocial))
                throw new ArgumentException("La razón social es obligatoria.", nameof(razonSocial));

            if (razonSocial.Length < 3)
                throw new ArgumentException("La razón social debe tener al menos 3 caracteres.", nameof(razonSocial));

            if (razonSocial.Length > 200)
                throw new ArgumentException("La razón social no puede exceder los 200 caracteres.", nameof(razonSocial));

            RazonSocial = razonSocial.Trim();
        }

        private void ValidarYEstablecerCUIT(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit))
                throw new ArgumentException("El CUIT es obligatorio.", nameof(cuit));

            var cuitLimpio = cuit.Replace("-", "").Replace(" ", "").Trim();

            if (cuitLimpio.Length != 11)
                throw new ArgumentException("El CUIT debe tener 11 dígitos.", nameof(cuit));

            foreach (char c in cuitLimpio)
            {
                if (!char.IsDigit(c))
                    throw new ArgumentException("El CUIT solo puede contener dígitos.", nameof(cuit));
            }

            if (!ValidarCUIT(cuitLimpio))
                throw new ArgumentException("El CUIT no es válido.", nameof(cuit));

            CUIT = cuitLimpio;
        }

        private bool ValidarCUIT(string numero)
        {
            if (numero.Length != 11) return false;

            int[] multiplicadores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int suma = 0;

            for (int i = 0; i < 10; i++)
            {
                suma += int.Parse(numero[i].ToString()) * multiplicadores[i];
            }

            int verificador = 11 - (suma % 11);
            if (verificador == 11) verificador = 0;
            if (verificador == 10) verificador = 9;

            return verificador == int.Parse(numero[10].ToString());
        }

        private void ValidarYEstablecerTipoIva(string tipoIva)
        {
            if (string.IsNullOrWhiteSpace(tipoIva))
                throw new ArgumentException("El tipo de IVA es obligatorio.", nameof(tipoIva));

            var tiposValidos = new[]
            {
                "RESPONSABLE INSCRIPTO",
                "MONOTRIBUTISTA",
                "EXENTO",
                "CONSUMIDOR FINAL",
                "NO RESPONSABLE"
            };

            var tipoUpper = tipoIva.ToUpper().Trim();

            if (Array.IndexOf(tiposValidos, tipoUpper) == -1)
                throw new ArgumentException($"El tipo de IVA debe ser uno de los siguientes: {string.Join(", ", tiposValidos)}", nameof(tipoIva));

            TipoIva = tipoUpper;
        }

        private void ValidarYEstablecerIdioma(string idioma)
        {
            if (string.IsNullOrWhiteSpace(idioma))
                throw new ArgumentException("El idioma es obligatorio.", nameof(idioma));

            var idiomasValidos = new[] { "es-AR", "en-US" };
            
            if (Array.IndexOf(idiomasValidos, idioma) == -1)
                throw new ArgumentException("El idioma debe ser 'es-AR' o 'en-US'.", nameof(idioma));

            Idioma = idioma;
        }

        /// <summary>
        /// Valida todos los datos de la configuración contra las reglas de negocio.
        /// 
        /// Se utiliza típicamente antes de persistir cambios.
        /// </summary>
        /// <exception cref="ArgumentException">Si alguna propiedad no cumple las reglas</exception>
        public void ValidarNegocio()
        {
            ValidarYEstablecerRazonSocial(RazonSocial);
            ValidarYEstablecerCUIT(CUIT);
            ValidarYEstablecerTipoIva(TipoIva);
            ValidarYEstablecerIdioma(Idioma);
        }
    }
}
