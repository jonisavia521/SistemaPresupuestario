using System;
using System.Text.RegularExpressions;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio Vendedor - Representa la lógica de negocio pura de un vendedor
    /// </summary>
    public class VendedorDM
    {
        public Guid Id { get; private set; }
        public string CodigoVendedor { get; private set; }
        public string Nombre { get; private set; }
        public string CUIT { get; private set; }
        public string Email { get; private set; }
        public string Telefono { get; private set; }
        public string Direccion { get; private set; }
        public decimal PorcentajeComision { get; private set; }
        public bool Activo { get; private set; }
        public DateTime FechaAlta { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        // Constructor para creación inicial
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

        // Constructor para cargar desde base de datos
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

        // Método de actualización
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

        public void ValidarNegocio()
        {
            ValidarYEstablecerNombre(Nombre);
            ValidarYEstablecerCUIT(CUIT);
            ValidarYEstablecerPorcentajeComision(PorcentajeComision);
        }
    }
}
