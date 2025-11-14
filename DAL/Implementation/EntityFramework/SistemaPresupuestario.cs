using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;

namespace DAL.Implementation.EntityFramework
{
    public partial class SistemaPresupuestario : DbContext
    {
        public SistemaPresupuestario()
            : base("name=SistemaPresupuestario")
        {
            // Habilitar el seguimiento de cambios y proxy de carga diferida
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }
        
        public SistemaPresupuestario(string connectionString) : base(connectionString)
        {
            // Habilitar el seguimiento de cambios y proxy de carga diferida
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }
        
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<Cliente_Impuesto> Cliente_Impuesto { get; set; }
        public virtual DbSet<Comprobante_Detalle> Comprobante_Detalle { get; set; }
        public virtual DbSet<Comprobantes> Comprobantes { get; set; }
        public virtual DbSet<Impuesto> Impuesto { get; set; }
        public virtual DbSet<ListaPrecio> ListaPrecio { get; set; }
        public virtual DbSet<ListaPrecio_Detalle> ListaPrecio_Detalle { get; set; }
        public virtual DbSet<Presupuesto> Presupuesto { get; set; }
        public virtual DbSet<Presupuesto_Detalle> Presupuesto_Detalle { get; set; }
        public virtual DbSet<Producto> Producto { get; set; }
        public virtual DbSet<Provincia> Provincia { get; set; }
        public virtual DbSet<TipoImpuesto> TipoImpuesto { get; set; }
        public virtual DbSet<Vendedor> Vendedor { get; set; }
        public virtual DbSet<Configuracion> Configuracion { get; set; } // NUEVO

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
                .Property(e => e.CodigoCliente)
                .IsUnicode(false);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.RazonSocial)
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
                .Property(e => e.TipoDocumento)
                .IsUnicode(false)
                .HasMaxLength(10);
            
            // Configuración de nuevos campos de Cliente
            // NOTA: CodigoVendedor fue eliminado - ahora se usa IdVendedor (FK)

            modelBuilder.Entity<Cliente>()
                .Property(e => e.TipoIva)
                .IsUnicode(false)
                .HasMaxLength(50);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.CondicionPago)
                .IsUnicode(false)
                .HasMaxLength(2);

            // NUEVO: Configuración de AlicuotaArba
            modelBuilder.Entity<Cliente>()
                .Property(e => e.AlicuotaArba)
                .IsRequired()
                .HasPrecision(18, 4);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.Email)
                .IsUnicode(false)
                .HasMaxLength(100);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.Telefono)
                .IsUnicode(false)
                .HasMaxLength(20);

            modelBuilder.Entity<Cliente>()
                .Property(e => e.Activo)
                .IsRequired();

            modelBuilder.Entity<Cliente>()
                .Property(e => e.FechaAlta)
                .IsRequired();

            modelBuilder.Entity<Cliente>()
                .HasMany(e => e.Comprobantes)
                .WithOptional(e => e.Cliente)
                .HasForeignKey(e => e.IdCliente);

            modelBuilder.Entity<Cliente>()
                .HasMany(e => e.Presupuesto)
                .WithRequired(e => e.Cliente)
                .HasForeignKey(e => e.IdCliente)
                .WillCascadeOnDelete(false);

            // NUEVO: Configuración de relación Cliente -> Provincia
            modelBuilder.Entity<Cliente>()
                .HasOptional(e => e.Provincia)
                .WithMany(p => p.Cliente)
                .HasForeignKey(e => e.IdProvincia);

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

            modelBuilder.Entity<Presupuesto>()
                .Property(e => e.Numero)
                .IsUnicode(false);

            // NUEVO: Configuración de ImporteArba
            modelBuilder.Entity<Presupuesto>()
                .Property(e => e.ImporteArba)
                .HasPrecision(18, 4);

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
                .ToTable("Producto");  // Especificar el nombre de la tabla

            modelBuilder.Entity<Producto>()
                .HasKey(e => e.ID); 

            modelBuilder.Entity<Producto>()
                .Property(e => e.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Producto>()
                .Property(e => e.Codigo)
                .IsRequired()       
                .HasMaxLength(50)  
                .IsUnicode(false)
                .HasColumnAnnotation("Index", new IndexAnnotation( 
        new IndexAttribute("IX_Producto_Codigo") { IsUnique = true }
    ));

            modelBuilder.Entity<Producto>()
                .Property(e => e.Descripcion)
                .IsOptional()       
                .HasMaxLength(50)  
                .IsUnicode(false);

            modelBuilder.Entity<Producto>()
                .Property(e => e.Inhabilitado)
                .IsRequired();      

            modelBuilder.Entity<Producto>()
                .Property(e => e.FechaAlta)
                .IsRequired();      

            modelBuilder.Entity<Producto>()
                .Property(e => e.UsuarioAlta)
                .IsRequired();     

            modelBuilder.Entity<Producto>()
                .Property(e => e.PorcentajeIVA)
                .IsRequired()
                .HasPrecision(5, 2);

            modelBuilder.Entity<Producto>()
                .HasMany(e => e.Comprobante_Detalle)
                .WithOptional(e => e.Producto)
                .HasForeignKey(e => e.IdProducto);

            modelBuilder.Entity<Producto>()
                .HasMany(e => e.Presupuesto_Detalle)
                .WithOptional(e => e.Producto)
                .HasForeignKey(e => e.IdProducto);

            // ? NUEVA CONFIGURACIÓN: Relación entre Producto y ListaPrecio_Detalle
            modelBuilder.Entity<Producto>()
                .HasMany(e => e.ListaPrecio_Detalle)
                .WithRequired(e => e.Producto)
                .HasForeignKey(e => e.IdProducto)
                .WillCascadeOnDelete(false);

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
            
            // Configuración de nuevos campos de Vendedor
            modelBuilder.Entity<Vendedor>()
                .Property(e => e.CodigoVendedor)
                .IsUnicode(false)
                .HasMaxLength(20);

            modelBuilder.Entity<Vendedor>()
                .Property(e => e.CUIT)
                .IsUnicode(false)
                .HasMaxLength(11);

            modelBuilder.Entity<Vendedor>()
                .Property(e => e.PorcentajeComision)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Vendedor>()
                .Property(e => e.Activo)
                .IsRequired();

            modelBuilder.Entity<Vendedor>()
                .Property(e => e.FechaAlta)
                .IsRequired();

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

            // Configuración de ListaPrecio
            modelBuilder.Entity<ListaPrecio>()
                .ToTable("ListaPrecio");

            modelBuilder.Entity<ListaPrecio>()
                .HasKey(e => e.ID);

            modelBuilder.Entity<ListaPrecio>()
                .Property(e => e.Codigo)
                .IsRequired()
                .HasMaxLength(2)
                .IsUnicode(false);

            modelBuilder.Entity<ListaPrecio>()
                .Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            modelBuilder.Entity<ListaPrecio>()
                .Property(e => e.Activo)
                .IsRequired();

            modelBuilder.Entity<ListaPrecio>()
                .Property(e => e.IncluyeIva)
                .IsRequired();

            modelBuilder.Entity<ListaPrecio>()
                .Property(e => e.FechaAlta)
                .IsRequired();

            // ✅ CAMBIO CRÍTICO: Habilitar eliminación en cascada para ListaPrecio_Detalle
            // Esto permite que Entity Framework elimine automáticamente los detalles huérfanos
            modelBuilder.Entity<ListaPrecio>()
                .HasMany(e => e.ListaPrecio_Detalle)
                .WithRequired(e => e.ListaPrecio)
                .HasForeignKey(e => e.IdListaPrecio)
                .WillCascadeOnDelete(true); // ⬅️ Cambiado de false a true

            modelBuilder.Entity<ListaPrecio>()
                .HasMany(e => e.Presupuesto)
                .WithOptional(e => e.ListaPrecio)
                .HasForeignKey(e => e.IdListaPrecio);

            // Configuración de ListaPrecio_Detalle
            modelBuilder.Entity<ListaPrecio_Detalle>()
                .ToTable("ListaPrecio_Detalle");

            modelBuilder.Entity<ListaPrecio_Detalle>()
                .HasKey(e => e.ID);

            modelBuilder.Entity<ListaPrecio_Detalle>()
                .Property(e => e.IdListaPrecio)
                .IsRequired();

            modelBuilder.Entity<ListaPrecio_Detalle>()
                .Property(e => e.IdProducto)
                .IsRequired();

            modelBuilder.Entity<ListaPrecio_Detalle>()
                .Property(e => e.Precio)
                .IsRequired()
                .HasPrecision(18, 4);

            // NUEVO: Configuración de Configuracion
            modelBuilder.Entity<Configuracion>()
                .ToTable("Configuracion");

            modelBuilder.Entity<Configuracion>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Configuracion>()
                .Property(e => e.RazonSocial)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);

            modelBuilder.Entity<Configuracion>()
                .Property(e => e.CUIT)
                .IsRequired()
                .HasMaxLength(11)
                .IsUnicode(false);

            modelBuilder.Entity<Configuracion>()
                .Property(e => e.TipoIva)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            modelBuilder.Entity<Configuracion>()
                .Property(e => e.Direccion)
                .HasMaxLength(200)
                .IsUnicode(false);

            modelBuilder.Entity<Configuracion>()
                .Property(e => e.Localidad)
                .HasMaxLength(100)
                .IsUnicode(false);

            modelBuilder.Entity<Configuracion>()
                .Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);

            modelBuilder.Entity<Configuracion>()
                .Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);

            modelBuilder.Entity<Configuracion>()
                .Property(e => e.Idioma)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);

            modelBuilder.Entity<Configuracion>()
                .Property(e => e.FechaAlta)
                .IsRequired();

            modelBuilder.Entity<Configuracion>()
                .HasOptional(e => e.Provincia)
                .WithMany()
                .HasForeignKey(e => e.IdProvincia);
        }
    }
}
