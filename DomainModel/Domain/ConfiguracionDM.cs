using System;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio Configuracion - Representa la configuración general del sistema.
    /// Esta tabla siempre tendrá un único registro.
    /// </summary>
    public class ConfiguracionDM
    {
        public Guid Id { get; private set; }
        public string RazonSocial { get; private set; }
        public string CUIT { get; private set; }
        public string TipoIva { get; private set; }
        public string Direccion { get; private set; }
        public string Localidad { get; private set; }
        public Guid? IdProvincia { get; private set; }
        public string Email { get; private set; }
        public string Telefono { get; private set; }
        public string Idioma { get; private set; }
        public DateTime FechaAlta { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        // Constructor para creación inicial
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

        // Constructor para cargar desde base de datos
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
        /// Actualiza todos los datos de la configuración
        /// </summary>
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
        /// Método principal de validación de negocio.
        /// Se puede invocar desde la BLL antes de persistir.
        /// </summary>
        public void ValidarNegocio()
        {
            ValidarYEstablecerRazonSocial(RazonSocial);
            ValidarYEstablecerCUIT(CUIT);
            ValidarYEstablecerTipoIva(TipoIva);
            ValidarYEstablecerIdioma(Idioma);
        }
    }
}
