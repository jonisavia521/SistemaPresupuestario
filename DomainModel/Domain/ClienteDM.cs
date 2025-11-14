using System;
using System.Text.RegularExpressions;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio Cliente - Representa la lógica de negocio pura de un cliente
    /// Esta clase es dueña de sus reglas de negocio y validaciones
    /// </summary>
    public class ClienteDM
    {
        public Guid Id { get; private set; }
        public string CodigoCliente { get; private set; }
        public string RazonSocial { get; private set; }
        public string TipoDocumento { get; private set; }
        public string NumeroDocumento { get; private set; }
        public Guid? IdVendedor { get; private set; }
        public Guid? IdProvincia { get; private set; }
        public string TipoIva { get; private set; }
        public string CondicionPago { get; private set; }
        public decimal AlicuotaArba { get; private set; }
        public string Email { get; private set; }
        public string Telefono { get; private set; }
        public string Direccion { get; private set; }
        public string Localidad { get; private set; }
        public bool Activo { get; private set; }
        public DateTime FechaAlta { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        // Constructor para creación inicial (nuevo cliente)
        public ClienteDM(
            string codigoCliente,
            string razonSocial,
            string tipoDocumento,
            string numeroDocumento,
            Guid? idVendedor,
            string tipoIva,
            string condicionPago,
            decimal alicuotaArba = 0,
            Guid? idProvincia = null,
            string email = null,
            string telefono = null,
            string direccion = null,
            string localidad = null)
        {
            Id = Guid.NewGuid();
            FechaAlta = DateTime.Now;
            Activo = true;

            ValidarYEstablecerCodigoCliente(codigoCliente);
            ValidarYEstablecerRazonSocial(razonSocial);
            ValidarYEstablecerTipoDocumento(tipoDocumento);
            ValidarYEstablecerNumeroDocumento(numeroDocumento, tipoDocumento);
            IdVendedor = idVendedor;
            IdProvincia = idProvincia;
            ValidarYEstablecerTipoIva(tipoIva);
            ValidarYEstablecerCondicionPago(condicionPago);
            ValidarYEstablecerAlicuotaArba(alicuotaArba);
            
            Email = email;
            Telefono = telefono;
            Direccion = direccion;
            Localidad = localidad;
        }

        // Constructor para cargar desde base de datos
        public ClienteDM(
            Guid id,
            string codigoCliente,
            string razonSocial,
            string tipoDocumento,
            string numeroDocumento,
            Guid? idVendedor,
            string tipoIva,
            string condicionPago,
            bool activo,
            DateTime fechaAlta,
            DateTime? fechaModificacion,
            decimal alicuotaArba = 0,
            Guid? idProvincia = null,
            string email = null,
            string telefono = null,
            string direccion = null,
            string localidad = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID del cliente no puede ser vacío.", nameof(id));

            Id = id;
            CodigoCliente = codigoCliente;
            RazonSocial = razonSocial;
            TipoDocumento = tipoDocumento;
            NumeroDocumento = numeroDocumento;
            IdVendedor = idVendedor;
            IdProvincia = idProvincia;
            TipoIva = tipoIva;
            CondicionPago = condicionPago;
            AlicuotaArba = alicuotaArba;
            Activo = activo;
            FechaAlta = fechaAlta;
            FechaModificacion = fechaModificacion;
            Email = email;
            Telefono = telefono;
            Direccion = direccion;
            Localidad = localidad;
        }

        // Método de actualización
        public void ActualizarDatos(
            string razonSocial,
            string tipoDocumento,
            string numeroDocumento,
            Guid? idVendedor,
            string tipoIva,
            string condicionPago,
            decimal alicuotaArba,
            Guid? idProvincia = null,
            string email = null,
            string telefono = null,
            string direccion = null,
            string localidad = null)
        {
            ValidarYEstablecerRazonSocial(razonSocial);
            ValidarYEstablecerTipoDocumento(tipoDocumento);
            ValidarYEstablecerNumeroDocumento(numeroDocumento, tipoDocumento);
            IdVendedor = idVendedor;
            IdProvincia = idProvincia;
            ValidarYEstablecerTipoIva(tipoIva);
            ValidarYEstablecerCondicionPago(condicionPago);
            ValidarYEstablecerAlicuotaArba(alicuotaArba);
            
            Email = email;
            Telefono = telefono;
            Direccion = direccion;
            Localidad = localidad;
            FechaModificacion = DateTime.Now;
        }

        public void Desactivar()
        {
            Activo = false;
            FechaModificacion = DateTime.Now;
        }

        public void Reactivar()
        {
            Activo = true;
            FechaModificacion = DateTime.Now;
        }

        /// <summary>
        /// Actualiza la alícuota ARBA del cliente.
        /// Usado para actualizaciones masivas del padrón de ARBA.
        /// </summary>
        public void ActualizarAlicuotaArba(decimal nuevaAlicuota)
        {
            ValidarYEstablecerAlicuotaArba(nuevaAlicuota);
            FechaModificacion = DateTime.Now;
        }

        // Validaciones de negocio
        private void ValidarYEstablecerCodigoCliente(string codigoCliente)
        {
            if (string.IsNullOrWhiteSpace(codigoCliente))
                throw new ArgumentException("El código de cliente es obligatorio.", nameof(codigoCliente));

            if (codigoCliente.Length > 20)
                throw new ArgumentException("El código de cliente no puede exceder los 20 caracteres.", nameof(codigoCliente));

            if (!Regex.IsMatch(codigoCliente, @"^[a-zA-Z0-9\-]+$"))
                throw new ArgumentException("El código de cliente solo puede contener letras, números y guiones.", nameof(codigoCliente));

            CodigoCliente = codigoCliente.ToUpper().Trim();
        }

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

        private void ValidarYEstablecerTipoDocumento(string tipoDocumento)
        {
            if (string.IsNullOrWhiteSpace(tipoDocumento))
                throw new ArgumentException("El tipo de documento es obligatorio.", nameof(tipoDocumento));

            var tipoUpper = tipoDocumento.ToUpper().Trim();
            
            if (tipoUpper != "DNI" && tipoUpper != "CUIT" && tipoUpper != "CUIL")
                throw new ArgumentException("El tipo de documento debe ser DNI, CUIT o CUIL.", nameof(tipoDocumento));

            TipoDocumento = tipoUpper;
        }

        private void ValidarYEstablecerNumeroDocumento(string numeroDocumento, string tipoDocumento)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento))
                throw new ArgumentException("El número de documento es obligatorio.", nameof(numeroDocumento));

            var numeroLimpio = numeroDocumento.Replace("-", "").Replace(" ", "").Trim();

            if (!Regex.IsMatch(numeroLimpio, @"^\d+$"))
                throw new ArgumentException("El número de documento solo puede contener dígitos.", nameof(numeroDocumento));

            switch (tipoDocumento.ToUpper())
            {
                case "DNI":
                    if (numeroLimpio.Length < 7 || numeroLimpio.Length > 8)
                        throw new ArgumentException("El DNI debe tener entre 7 y 8 dígitos.", nameof(numeroDocumento));
                    break;

                case "CUIT":
                case "CUIL":
                    if (numeroLimpio.Length != 11)
                        throw new ArgumentException($"El {tipoDocumento} debe tener 11 dígitos.", nameof(numeroDocumento));
                    
                    if (!ValidarCUITCUIL(numeroLimpio))
                        throw new ArgumentException($"El {tipoDocumento} no es válido.", nameof(numeroDocumento));
                    break;
            }

            NumeroDocumento = numeroLimpio;
        }

        private bool ValidarCUITCUIL(string numero)
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

        private void ValidarYEstablecerCondicionPago(string condicionPago)
        {
            if (string.IsNullOrWhiteSpace(condicionPago))
                throw new ArgumentException("La condición de pago es obligatoria.", nameof(condicionPago));

            if (!Regex.IsMatch(condicionPago, @"^\d{2}$"))
                throw new ArgumentException("La condición de pago debe tener exactamente 2 dígitos numéricos.", nameof(condicionPago));

            CondicionPago = condicionPago;
        }

        private void ValidarYEstablecerAlicuotaArba(decimal alicuotaArba)
        {
            if (alicuotaArba < 0)
                throw new ArgumentException("La alícuota ARBA no puede ser negativa.", nameof(alicuotaArba));

            if (alicuotaArba > 100)
                throw new ArgumentException("La alícuota ARBA no puede ser mayor a 100.", nameof(alicuotaArba));

            AlicuotaArba = alicuotaArba;
        }

        /// <summary>
        /// Método principal de validación de negocio.
        /// Se puede invocar desde la BLL antes de persistir.
        /// </summary>
        public void ValidarNegocio()
        {
            ValidarYEstablecerRazonSocial(RazonSocial);
            ValidarYEstablecerTipoDocumento(TipoDocumento);
            ValidarYEstablecerNumeroDocumento(NumeroDocumento, TipoDocumento);
            ValidarYEstablecerTipoIva(TipoIva);
            ValidarYEstablecerCondicionPago(CondicionPago);
            ValidarYEstablecerAlicuotaArba(AlicuotaArba);
        }
    }
}
