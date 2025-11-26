using System;
using System.Text.RegularExpressions;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio que representa un cliente del sistema.
    /// 
    /// Esta clase encapsula la lógica de negocio pura relacionada con los clientes,
    /// incluyendo validaciones de datos fiscales y comerciales. Es la responsable
    /// de mantener la consistencia de los datos del cliente según las reglas de negocio
    /// definidas para el sistema de presupuestos.
    /// 
    /// Responsabilidades:
    /// - Validar datos fiscales (CUIT, DNI, CUIL) con algoritmo verificador
    /// - Validar tipos de IVA según categorías AFIP
    /// - Mantener auditoría de creación y modificación
    /// - Gestionar estado de activación/desactivación
    /// - Calcular y actualizar alícuota ARBA
    /// 
    /// Invariantes:
    /// - El código de cliente es único y alfanumérico
    /// - La razón social tiene entre 3 y 200 caracteres
    /// - El tipo de IVA debe ser una categoría válida según AFIP
    /// - Los números de documento son validados según tipo
    /// 
    /// Ejemplo de uso:
    /// <code>
    /// var cliente = new ClienteDM(
    ///     "CLI001",
    ///     "Acme Corporation S.A.",
    ///     "CUIT",
    ///     "20345678905",
    ///     null,
    ///     "RESPONSABLE INSCRIPTO",
    ///     "01");
    /// </code>
    /// </summary>
    public class ClienteDM
    {
        /// <summary>Identificador único del cliente en el sistema</summary>
        public Guid Id { get; private set; }
        
        /// <summary>Código alfanumérico único para referencia comercial. Máximo 20 caracteres.</summary>
        public string CodigoCliente { get; private set; }
        
        /// <summary>Razón social o nombre completo del cliente. Entre 3 y 200 caracteres.</summary>
        public string RazonSocial { get; private set; }
        
        /// <summary>Tipo de documento: DNI, CUIT o CUIL</summary>
        public string TipoDocumento { get; private set; }
        
        /// <summary>Número de documento sin formatear (solo dígitos)</summary>
        public string NumeroDocumento { get; private set; }
        
        /// <summary>Identificador del vendedor asignado al cliente (opcional)</summary>
        public Guid? IdVendedor { get; private set; }
        
        /// <summary>Identificador de la provincia donde se localiza el cliente</summary>
        public Guid? IdProvincia { get; private set; }
        
        /// <summary>Categoría de IVA según AFIP: RESPONSABLE INSCRIPTO, MONOTRIBUTISTA, EXENTO, CONSUMIDOR FINAL, NO RESPONSABLE</summary>
        public string TipoIva { get; private set; }
        
        /// <summary>Código de condición de pago (2 dígitos numéricos)</summary>
        public string CondicionPago { get; private set; }
        
        /// <summary>Alícuota IIBB ARBA aplicable al cliente, entre 0 y 100</summary>
        public decimal AlicuotaArba { get; private set; }
        
        /// <summary>Dirección de correo electrónico del cliente (opcional)</summary>
        public string Email { get; private set; }
        
        /// <summary>Número de teléfono de contacto (opcional)</summary>
        public string Telefono { get; private set; }
        
        /// <summary>Dirección postal completa del cliente (opcional)</summary>
        public string Direccion { get; private set; }
        
        /// <summary>Localidad o ciudad donde se ubica el cliente (opcional)</summary>
        public string Localidad { get; private set; }
        
        /// <summary>Indica si el cliente está activo en el sistema. Los inactivos no aparecen en búsquedas estándar.</summary>
        public bool Activo { get; private set; }
        
        /// <summary>Fecha y hora en que se registró el cliente en el sistema</summary>
        public DateTime FechaAlta { get; private set; }
        
        /// <summary>Fecha y hora de la última modificación del registro. Nula si nunca fue modificado.</summary>
        public DateTime? FechaModificacion { get; private set; }

        /// <summary>
        /// Crea una nueva instancia de cliente para registrar en el sistema.
        /// 
        /// Se validan automáticamente todos los parámetros según las reglas de negocio.
        /// El cliente se crea activo con fecha de alta en el momento actual.
        /// </summary>
        /// <param name="codigoCliente">Código único del cliente (alfanumérico, máximo 20 caracteres)</param>
        /// <param name="razonSocial">Nombre o razón social (entre 3 y 200 caracteres)</param>
        /// <param name="tipoDocumento">Tipo: DNI, CUIT o CUIL</param>
        /// <param name="numeroDocumento">Número sin formatear (7-8 dígitos para DNI, 11 para CUIT/CUIL)</param>
        /// <param name="idVendedor">GUID del vendedor asignado (opcional)</param>
        /// <param name="tipoIva">Categoría AFIP (RESPONSABLE INSCRIPTO, MONOTRIBUTISTA, etc.)</param>
        /// <param name="condicionPago">Código de 2 dígitos para condición de pago</param>
        /// <param name="alicuotaArba">Alícuota IIBB ARBA entre 0 y 100 (predeterminado: 0)</param>
        /// <param name="idProvincia">GUID de la provincia (opcional)</param>
        /// <param name="email">Correo electrónico (opcional)</param>
        /// <param name="telefono">Número de teléfono (opcional)</param>
        /// <param name="direccion">Dirección postal completa (opcional)</param>
        /// <param name="localidad">Localidad o ciudad (opcional)</param>
        /// <exception cref="ArgumentException">Si alguno de los parámetros no cumple con las validaciones de negocio</exception>
        /// <exception cref="ArgumentNullException">Si un parámetro requerido es nulo</exception>
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

        /// <summary>
        /// Constructor para hidratar una instancia de cliente desde la base de datos.
        /// 
        /// Este constructor se utiliza internamente por los repositorios para construir
        /// objetos desde registros persistidos. Realiza validación mínima asumiendo
        /// que los datos ya fueron validados durante su creación/actualización.
        /// </summary>
        /// <param name="id">GUID único del cliente en la base de datos</param>
        /// <param name="codigoCliente">Código del cliente</param>
        /// <param name="razonSocial">Razón social del cliente</param>
        /// <param name="tipoDocumento">Tipo de documento</param>
        /// <param name="numeroDocumento">Número de documento</param>
        /// <param name="idVendedor">GUID del vendedor (puede ser nulo)</param>
        /// <param name="tipoIva">Tipo de IVA</param>
        /// <param name="condicionPago">Condición de pago</param>
        /// <param name="activo">Estado de activación</param>
        /// <param name="fechaAlta">Fecha de creación del registro</param>
        /// <param name="fechaModificacion">Fecha de última modificación (puede ser nula)</param>
        /// <param name="alicuotaArba">Alícuota ARBA aplicable</param>
        /// <param name="idProvincia">GUID de la provincia (puede ser nulo)</param>
        /// <param name="email">Correo electrónico del cliente (puede ser nulo)</param>
        /// <param name="telefono">Teléfono del cliente (puede ser nulo)</param>
        /// <param name="direccion">Dirección del cliente (puede ser nula)</param>
        /// <param name="localidad">Localidad del cliente (puede ser nula)</param>
        /// <exception cref="ArgumentException">Si el ID es un GUID vacío</exception>
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

        /// <summary>
        /// Actualiza los datos del cliente manteniendo la auditoría de cambios.
        /// 
        /// Este método valida todos los parámetros según las reglas de negocio y
        /// actualiza la fecha de modificación al momento actual. No modifica el
        /// código de cliente (es inmutable) ni el estado de activación.
        /// </summary>
        /// <param name="razonSocial">Nueva razón social del cliente</param>
        /// <param name="tipoDocumento">Nuevo tipo de documento</param>
        /// <param name="numeroDocumento">Nuevo número de documento</param>
        /// <param name="idVendedor">Nuevo vendedor asignado (puede ser nulo)</param>
        /// <param name="tipoIva">Nueva categoría de IVA</param>
        /// <param name="condicionPago">Nueva condición de pago</param>
        /// <param name="alicuotaArba">Nueva alícuota ARBA</param>
        /// <param name="idProvincia">Nueva provincia (puede ser nula)</param>
        /// <param name="email">Nuevo correo electrónico (puede ser nulo)</param>
        /// <param name="telefono">Nuevo teléfono (puede ser nulo)</param>
        /// <param name="direccion">Nueva dirección (puede ser nula)</param>
        /// <param name="localidad">Nueva localidad (puede ser nula)</param>
        /// <exception cref="ArgumentException">Si algún parámetro no cumple validaciones</exception>
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

        /// <summary>
        /// Desactiva el cliente lógicamente sin eliminar el registro de la base de datos.
        /// 
        /// Los clientes desactivados no aparecen en búsquedas normales pero pueden
        /// ser reactivados. Mantiene historial completo de transacciones y presupuestos.
        /// </summary>
        public void Desactivar()
        {
            Activo = false;
            FechaModificacion = DateTime.Now;
        }

        /// <summary>
        /// Reactiva un cliente previamente desactivado.
        /// 
        /// Permite que el cliente vuelva a aparecer en búsquedas normales y pueda
        /// recibir nuevos presupuestos.
        /// </summary>
        public void Reactivar()
        {
            Activo = true;
            FechaModificacion = DateTime.Now;
        }

        /// <summary>
        /// Actualiza exclusivamente la alícuota IIBB ARBA del cliente.
        /// 
        /// Este método se utiliza en procesos de actualización masiva del padrón ARBA
        /// proporcionado por la provincia. Valida que la nueva alícuota esté en rango
        /// permitido (0-100) y actualiza la fecha de modificación.
        /// </summary>
        /// <param name="nuevaAlicuota">Nueva alícuota ARBA entre 0 y 100</param>
        /// <exception cref="ArgumentException">Si la alícuota está fuera del rango permitido</exception>
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
        /// Valida todos los datos del cliente contra las reglas de negocio.
        /// 
        /// Este método consolida todas las validaciones individuales de propiedades
        /// en una única llamada. Se utiliza típicamente en la capa BLL antes de
        /// persistir cambios a la base de datos.
        /// </summary>
        /// <exception cref="ArgumentException">Si alguna propiedad no cumple las reglas de negocio</exception>
        /// <remarks>
        /// Las validaciones incluyen:
        /// - Formato y longitud de código y razón social
        /// - Validación de dígitos verificadores de documentos
        /// - Categorías de IVA válidas según AFIP
        /// - Formato de condición de pago
        /// - Rango de alícuota ARBA
        /// </remarks>
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
