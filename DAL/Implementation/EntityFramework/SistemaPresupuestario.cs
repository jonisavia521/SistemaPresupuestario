using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DAL.Implementation.EntityFramework
{
    public partial class SistemaPresupuestario : DbContext
    {
        public SistemaPresupuestario()
            : base("name=SistemaPresupuestario")
        {
        }
        public SistemaPresupuestario(string connectionString) : base(connectionString)
        {
        }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<Cliente_Impuesto> Cliente_Impuesto { get; set; }
        public virtual DbSet<Comprobante_Detalle> Comprobante_Detalle { get; set; }
        public virtual DbSet<Comprobantes> Comprobantes { get; set; }
        public virtual DbSet<Familia> Familia { get; set; }
        public virtual DbSet<Familia_Familia> Familia_Familia { get; set; }
        public virtual DbSet<Familia_Patente> Familia_Patente { get; set; }
        public virtual DbSet<Impuesto> Impuesto { get; set; }
        public virtual DbSet<Patente> Patente { get; set; }
        public virtual DbSet<Presupuesto> Presupuesto { get; set; }
        public virtual DbSet<Presupuesto_Detalle> Presupuesto_Detalle { get; set; }
        public virtual DbSet<Producto> Producto { get; set; }
        public virtual DbSet<Provincia> Provincia { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TipoImpuesto> TipoImpuesto { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<Usuario_Familia> Usuario_Familia { get; set; }
        public virtual DbSet<Usuario_Patente> Usuario_Patente { get; set; }
        public virtual DbSet<Vendedor> Vendedor { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
                .Property(e => e.CodigoCliente)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.RazonSocial)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.IdProvincia)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.Localidad)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.DireccionLegal)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.DireccionComercial)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.CUIT)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .HasMany(e => e.Comprobantes)
                .WithOptional(e => e.Cliente)
                .HasForeignKey(e => e.IdCliente);

            modelBuilder.Entity<Cliente>()
                .HasMany(e => e.Presupuesto)
                .WithRequired(e => e.Cliente)
                .HasForeignKey(e => e.IdCliente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Comprobante_Detalle>()
                .Property(e => e.TipoComprobante)
                .IsUnicode(false);

            modelBuilder.Entity<Comprobante_Detalle>()
                .Property(e => e.NumeroComprobante)
                .IsUnicode(false);

            modelBuilder.Entity<Comprobante_Detalle>()
                .Property(e => e.Renglon)
                .IsUnicode(false);

            modelBuilder.Entity<Comprobante_Detalle>()
                .Property(e => e.Cantidad)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Comprobante_Detalle>()
                .Property(e => e.ImporteNeto)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Comprobante_Detalle>()
                .Property(e => e.PrecioNeto)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Comprobantes>()
                .Property(e => e.Estado)
                .IsUnicode(false);

            modelBuilder.Entity<Comprobantes>()
                .Property(e => e.TipoComprobante)
                .IsUnicode(false);

            modelBuilder.Entity<Comprobantes>()
                .Property(e => e.NumeroComprobante)
                .IsUnicode(false);

            modelBuilder.Entity<Comprobantes>()
                .Property(e => e.Total)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Comprobantes>()
                .Property(e => e.ImporteGravado)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Comprobantes>()
                .Property(e => e.ImporteIVA)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Comprobantes>()
                .HasMany(e => e.Comprobante_Detalle)
                .WithRequired(e => e.Comprobantes)
                .HasForeignKey(e => e.IdComprobante)
                .WillCascadeOnDelete(false);

            // (Todo lo que ya tienes ANTES de la sección de Familia se mantiene igual)

            modelBuilder.Entity<Familia>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Familia>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            // Quita las configuraciones antiguas:
            // (Elimina los dos HasMany originales que causaban el conflicto)

            // Nueva configuración clara desde la entidad intermedia
            modelBuilder.Entity<Familia_Familia>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<Familia_Familia>()
                .HasRequired(ff => ff.FamiliaPadre)
                .WithMany(f => f.RelacionesComoPadre)
                .HasForeignKey(ff => ff.IdFamilia)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Familia_Familia>()
                .HasRequired(ff => ff.FamiliaHijo)
                .WithMany(f => f.RelacionesComoHijo)
                .HasForeignKey(ff => ff.IdFamiliaHijo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Familia>()
                .HasMany(e => e.Familia_Patente)
                .WithRequired(e => e.Familia)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Familia>()
                .HasMany(e => e.Usuario_Familia)
                .WithRequired(e => e.Familia)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Familia_Patente>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<Impuesto>()
                .Property(e => e.Codigo)
                .IsUnicode(false);

            modelBuilder.Entity<Impuesto>()
                .Property(e => e.Descripcion)
                .IsUnicode(false);

            modelBuilder.Entity<Impuesto>()
                .HasMany(e => e.Cliente_Impuesto)
                .WithRequired(e => e.Impuesto)
                .HasForeignKey(e => e.IdImpuesto)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Patente>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Patente>()
                .Property(e => e.Vista)
                .IsUnicode(false);

            modelBuilder.Entity<Patente>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<Patente>()
                .HasMany(e => e.Familia_Patente)
                .WithRequired(e => e.Patente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Patente>()
                .HasMany(e => e.Usuario_Patente)
                .WithRequired(e => e.Patente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Presupuesto>()
                .Property(e => e.Numero)
                .IsUnicode(false);

            modelBuilder.Entity<Presupuesto>()
                .HasMany(e => e.Presupuesto_Detalle)
                .WithOptional(e => e.Presupuesto)
                .HasForeignKey(e => e.IdPresupuesto);

            modelBuilder.Entity<Presupuesto>()
                .HasMany(e => e.Presupuesto1)
                .WithOptional(e => e.Presupuesto2)
                .HasForeignKey(e => e.IdPresupuestoPadre);

            modelBuilder.Entity<Presupuesto_Detalle>()
                .Property(e => e.Numero)
                .IsUnicode(false);

            modelBuilder.Entity<Presupuesto_Detalle>()
                .Property(e => e.Cantidad)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Presupuesto_Detalle>()
                .Property(e => e.Precio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Presupuesto_Detalle>()
                .Property(e => e.Descuento)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Producto>()
                .Property(e => e.Codigo)
                .IsUnicode(false);

            modelBuilder.Entity<Producto>()
                .Property(e => e.Descripcion)
                .IsUnicode(false);

            modelBuilder.Entity<Producto>()
                .HasMany(e => e.Comprobante_Detalle)
                .WithOptional(e => e.Producto)
                .HasForeignKey(e => e.IdProducto);

            modelBuilder.Entity<Producto>()
                .HasMany(e => e.Presupuesto_Detalle)
                .WithOptional(e => e.Producto)
                .HasForeignKey(e => e.IdProducto);

            modelBuilder.Entity<Provincia>()
                .Property(e => e.CodigoAFIP)
                .IsUnicode(false);

            modelBuilder.Entity<Provincia>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Provincia>()
                .HasMany(e => e.Impuesto)
                .WithOptional(e => e.Provincia)
                .HasForeignKey(e => e.IdProvincia);

            modelBuilder.Entity<TipoImpuesto>()
                .Property(e => e.Descripcion)
                .IsUnicode(false);

            modelBuilder.Entity<TipoImpuesto>()
                .HasMany(e => e.Cliente_Impuesto)
                .WithRequired(e => e.TipoImpuesto)
                .HasForeignKey(e => e.IdTipoImpuesto)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TipoImpuesto>()
                .HasMany(e => e.Impuesto)
                .WithRequired(e => e.TipoImpuesto)
                .HasForeignKey(e => e.IdTipoImpuesto)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Usuario1)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Clave)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.Usuario_Familia)
                .WithRequired(e => e.Usuario)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.Usuario_Patente)
                .WithRequired(e => e.Usuario)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario_Familia>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<Usuario_Patente>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<Vendedor>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Vendedor>()
                .Property(e => e.Mail)
                .IsUnicode(false);

            modelBuilder.Entity<Vendedor>()
                .Property(e => e.Direccion)
                .IsUnicode(false);

            modelBuilder.Entity<Vendedor>()
                .Property(e => e.Telefono)
                .IsUnicode(false);

            modelBuilder.Entity<Vendedor>()
                .HasMany(e => e.Cliente)
                .WithOptional(e => e.Vendedor)
                .HasForeignKey(e => e.IdVendedor);

            modelBuilder.Entity<Vendedor>()
                .HasMany(e => e.Comprobantes)
                .WithOptional(e => e.Vendedor)
                .HasForeignKey(e => e.IdVendedor);

            modelBuilder.Entity<Vendedor>()
                .HasMany(e => e.Presupuesto)
                .WithOptional(e => e.Vendedor)
                .HasForeignKey(e => e.IdVendedor);
        }
    }
}
