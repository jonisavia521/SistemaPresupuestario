using System;
using System.Text.RegularExpressions;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio que representa un vendedor del sistema.
    /// 
    /// Esta clase encapsula la lógica de negocio pura relacionada con los vendedores,
    /// personal de ventas que genera presupuestos y cotizaciones. Incluye validaciones
    /// de datos personales, CUIT y comisiones.
    /// 
    /// Responsabilidades:
    /// - Validar código único de 2 dígitos
    /// - Validar CUIT con algoritmo verificador
    /// - Validar porcentaje de comisión entre 0 y 100
    /// - Mantener auditoría de creación y modificación
    /// - Gestionar estado de activación/desactivación
    /// 
    /// Invariantes:
    /// - El código es exactamente 2 dígitos numéricos
    /// - El nombre tiene entre 3 y 200 caracteres
    /// - El CUIT es válido según algoritmo AFIP (11 dígitos)
    /// - El porcentaje de comisión está entre 0 y 100
    /// 
    /// Estados:
    /// - Activo: Vendedor disponible para asignar a presupuestos
    /// - Inactivo: Vendedor no disponible, no aparece en listas de selección
    /// 
    /// Ejemplo de uso:
    /// <code>
    /// var vendedor = new VendedorDM(
    ///     "01",
    ///     "Juan Pérez",
    ///     "20345678905",
    ///     15.50m,
    ///     "juan.perez@empresa.com");
    /// </code>
    /// </summary>
    public class VendedorDM
    {
        /// <summary>Identificador único del vendedor en el sistema</summary>
        public Guid Id { get; private set; }
        
        /// <summary>Código numérico único del vendedor. Exactamente 2 dígitos.</summary>
        public string CodigoVendedor { get; private set; }
        
        /// <summary>Nombre completo del vendedor. Entre 3 y 200 caracteres.</summary>
        public string Nombre { get; private set; }
        
        /// <summary>Número CUIT del vendedor (11 dígitos validados con algoritmo AFIP)</summary>
        public string CUIT { get; private set; }
        
        /// <summary>Correo electrónico de contacto del vendedor (opcional)</summary>
        public string Email { get; private set; }
        
        /// <summary>Número de teléfono del vendedor (opcional)</summary>
        public string Telefono { get; private set; }
        
        /// <summary>Dirección postal del vendedor (opcional)</summary>
        public string Direccion { get; private set; }
        
        /// <summary>Porcentaje de comisión sobre ventas realizadas. Entre 0 y 100.</summary>
        public decimal PorcentajeComision { get; private set; }
        
        /// <summary>Indica si el vendedor está activo en el sistema</summary>
        public bool Activo { get; private set; }
        
        /// <summary>Fecha y hora en que se registró el vendedor en el sistema</summary>
        public DateTime FechaAlta { get; private set; }
        
        /// <summary>Fecha y hora de la última modificación del registro. Nula si nunca fue modificado.</summary>
        public DateTime? FechaModificacion { get; private set; }

        /// <summary>
        /// Crea una nueva instancia de vendedor para registrar en el sistema.
        /// 
        /// Se validan automáticamente todos los parámetros según las reglas de negocio.
        /// El vendedor se crea activo con fecha de alta en el momento actual.
        /// </summary>
        /// <param name="codigoVendedor">Código único de exactamente 2 dígitos numéricos</param>
        /// <param name="nombre">Nombre completo (entre 3 y 200 caracteres)</param>
        /// <param name="cuit">Número CUIT de 11 dígitos con dígito verificador</param>
        /// <param name="porcentajeComision">Porcentaje de comisión entre 0 y 100</param>
        /// <param name="email">Correo electrónico (opcional)</param>
        /// <param name="telefono">Teléfono de contacto (opcional)</param>
        /// <param name="direccion">Dirección postal (opcional)</param>
        /// <exception cref="ArgumentException">Si algún parámetro no cumple con las validaciones</exception>
        /// <exception cref="ArgumentNullException">Si un parámetro requerido es nulo</exception>
        public VendedorDM(
            string codigoVendedor,
            string nombre,
            string cuit,
            decimal porcentajeComision,
            string email = null,
            string telefono = null,
            string direccion = null)
        {
            Id = Guid.NewGuid();
            FechaAlta = DateTime.Now;
            Activo = true;

            ValidarYEstablecerCodigoVendedor(codigoVendedor);
            ValidarYEstablecerNombre(nombre);
            ValidarYEstablecerCUIT(cuit);
            ValidarYEstablecerPorcentajeComision(porcentajeComision);
            
            Email = email;
            Telefono = telefono;
            Direccion = direccion;
        }

        /// <summary>
        /// Constructor para hidratar una instancia de vendedor desde la base de datos.
        /// 
        /// Este constructor se utiliza internamente por los repositorios para construir
        /// objetos desde registros persistidos.
        /// </summary>
        /// <param name="id">GUID único del vendedor en la base de datos</param>
        /// <param name="codigoVendedor">Código del vendedor</param>
        /// <param name="nombre">Nombre del vendedor</param>
        /// <param name="cuit">CUIT del vendedor</param>
        /// <param name="porcentajeComision">Porcentaje de comisión</param>
        /// <param name="activo">Estado de activación</param>
        /// <param name="fechaAlta">Fecha de creación del registro</param>
        /// <param name="fechaModificacion">Fecha de última modificación (puede ser nula)</param>
        /// <param name="email">Correo electrónico (puede ser nulo)</param>
        /// <param name="telefono">Teléfono (puede ser nulo)</param>
        /// <param name="direccion">Dirección (puede ser nula)</param>
        /// <exception cref="ArgumentException">Si el ID es un GUID vacío</exception>
        public VendedorDM(
            Guid id,
            string codigoVendedor,
            string nombre,
            string cuit,
            decimal porcentajeComision,
            bool activo,
            DateTime fechaAlta,
            DateTime? fechaModificacion,
            string email = null,
            string telefono = null,
            string direccion = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID del vendedor no puede ser vacío.", nameof(id));

            Id = id;
            CodigoVendedor = codigoVendedor;
            Nombre = nombre;
            CUIT = cuit;
            PorcentajeComision = porcentajeComision;
            Activo = activo;
            FechaAlta = fechaAlta;
            FechaModificacion = fechaModificacion;
            Email = email;
            Telefono = telefono;
            Direccion = direccion;
        }

        /// <summary>
        /// Actualiza los datos del vendedor manteniendo la auditoría de cambios.
        /// 
        /// Este método valida todos los parámetros según las reglas de negocio y
        /// actualiza la fecha de modificación. No modifica el código (es inmutable)
        /// ni el estado de activación.
        /// </summary>
        /// <param name="nombre">Nuevo nombre del vendedor</param>
        /// <param name="cuit">Nuevo CUIT del vendedor</param>
        /// <param name="porcentajeComision">Nuevo porcentaje de comisión</param>
        /// <param name="email">Nuevo correo electrónico (puede ser nulo)</param>
        /// <param name="telefono">Nuevo teléfono (puede ser nulo)</param>
        /// <param name="direccion">Nueva dirección (puede ser nula)</param>
        /// <exception cref="ArgumentException">Si algún parámetro no cumple validaciones</exception>
        public void ActualizarDatos(
            string nombre,
            string cuit,
            decimal porcentajeComision,
            string email = null,
            string telefono = null,
            string direccion = null)
        {
            ValidarYEstablecerNombre(nombre);
            ValidarYEstablecerCUIT(cuit);
            ValidarYEstablecerPorcentajeComision(porcentajeComision);
            
            Email = email;
            Telefono = telefono;
            Direccion = direccion;
            FechaModificacion = DateTime.Now;
        }

        /// <summary>
        /// Desactiva el vendedor lógicamente sin eliminar el registro de la base de datos.
        /// 
        /// Los vendedores desactivados no aparecen en listas de selección pero mantienen
        /// historial de presupuestos realizados.
        /// </summary>
        public void Desactivar()
        {
            Activo = false;
            FechaModificacion = DateTime.Now;
        }

        /// <summary>
        /// Reactiva un vendedor previamente desactivado.
        /// 
        /// Permite que el vendedor vuelva a aparecer en listas de selección y pueda
        /// recibir nuevos presupuestos.
        /// </summary>
        public void Reactivar()
        {
            Activo = true;
            FechaModificacion = DateTime.Now;
        }

        // Validaciones de negocio
        private void ValidarYEstablecerCodigoVendedor(string codigoVendedor)
        {
            if (string.IsNullOrWhiteSpace(codigoVendedor))
                throw new ArgumentException("El código de vendedor es obligatorio.", nameof(codigoVendedor));

            if (!Regex.IsMatch(codigoVendedor, @"^\d{2}$"))
                throw new ArgumentException("El código de vendedor debe tener exactamente 2 dígitos numéricos.", nameof(codigoVendedor));

            CodigoVendedor = codigoVendedor;
        }

        private void ValidarYEstablecerNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

            if (nombre.Length < 3)
                throw new ArgumentException("El nombre debe tener al menos 3 caracteres.", nameof(nombre));

            if (nombre.Length > 200)
                throw new ArgumentException("El nombre no puede exceder los 200 caracteres.", nameof(nombre));

            Nombre = nombre.Trim();
        }

        private void ValidarYEstablecerCUIT(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit))
                throw new ArgumentException("El CUIT es obligatorio.", nameof(cuit));

            var cuitLimpio = cuit.Replace("-", "").Replace(" ", "").Trim();

            if (!Regex.IsMatch(cuitLimpio, @"^\d+$"))
                throw new ArgumentException("El CUIT solo puede contener dígitos.", nameof(cuit));

            if (cuitLimpio.Length != 11)
                throw new ArgumentException("El CUIT debe tener 11 dígitos.", nameof(cuit));

            if (!ValidarFormatoCUIT(cuitLimpio))
                throw new ArgumentException("El CUIT no es válido.", nameof(cuit));

            CUIT = cuitLimpio;
        }

        private bool ValidarFormatoCUIT(string cuit)
        {
            if (cuit.Length != 11) return false;

            int[] multiplicadores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int suma = 0;

            for (int i = 0; i < 10; i++)
            {
                suma += int.Parse(cuit[i].ToString()) * multiplicadores[i];
            }

            int verificador = 11 - (suma % 11);
            if (verificador == 11) verificador = 0;
            if (verificador == 10) verificador = 9;

            return verificador == int.Parse(cuit[10].ToString());
        }

        private void ValidarYEstablecerPorcentajeComision(decimal porcentajeComision)
        {
            if (porcentajeComision < 0 || porcentajeComision > 100)
                throw new ArgumentException("El porcentaje de comisión debe estar entre 0 y 100.", nameof(porcentajeComision));

            PorcentajeComision = porcentajeComision;
        }

        /// <summary>
        /// Valida todos los datos del vendedor contra las reglas de negocio.
        /// 
        /// Este método consolida todas las validaciones individuales de propiedades
        /// en una única llamada. Se utiliza típicamente en la capa BLL antes de
        /// persistir cambios a la base de datos.
        /// </summary>
        /// <exception cref="ArgumentException">Si alguna propiedad no cumple las reglas de negocio</exception>
        /// <remarks>
        /// Las validaciones incluyen:
        /// - Nombre: entre 3 y 200 caracteres
        /// - CUIT: 11 dígitos con dígito verificador válido
        /// - Comisión: entre 0 y 100
        /// </remarks>
        public void ValidarNegocio()
        {
            ValidarYEstablecerNombre(Nombre);
            ValidarYEstablecerCUIT(CUIT);
            ValidarYEstablecerPorcentajeComision(PorcentajeComision);
        }
    }
}
