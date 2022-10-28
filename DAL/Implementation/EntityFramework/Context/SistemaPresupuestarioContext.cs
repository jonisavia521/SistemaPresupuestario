using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class SistemaPresupuestarioContext : DbContext
    {
        public SistemaPresupuestarioContext()
        {
        }

        public SistemaPresupuestarioContext(DbContextOptions<SistemaPresupuestarioContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<ClienteImpuesto> ClienteImpuesto { get; set; }
        public virtual DbSet<ComprobanteDetalle> ComprobanteDetalle { get; set; }
        public virtual DbSet<Comprobantes> Comprobantes { get; set; }
        public virtual DbSet<Familia> Familia { get; set; }
        public virtual DbSet<FamiliaFamilia> FamiliaFamilia { get; set; }
        public virtual DbSet<FamiliaPatente> FamiliaPatente { get; set; }
        public virtual DbSet<Impuesto> Impuesto { get; set; }
        public virtual DbSet<Patente> Patente { get; set; }
        public virtual DbSet<Presupuesto> Presupuesto { get; set; }
        public virtual DbSet<PresupuestoDetalle> PresupuestoDetalle { get; set; }
        public virtual DbSet<Producto> Producto { get; set; }
        public virtual DbSet<Provincia> Provincia { get; set; }
        public virtual DbSet<TipoImpuesto> TipoImpuesto { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<UsuarioFamilia> UsuarioFamilia { get; set; }
        public virtual DbSet<UsuarioPatente> UsuarioPatente { get; set; }
        public virtual DbSet<Vendedor> Vendedor { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("server=proyectosfacultad.database.windows.net,1433;  INITIAL CATALOG=SistemaPresupuestario; User Id=Axoft;Password=Programas521?;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CodigoCliente)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cuit)
                    .HasColumnName("CUIT")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DireccionComercial)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DireccionLegal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdProvincia)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Localidad)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RazonSocial)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdVendedorNavigation)
                    .WithMany(p => p.Cliente)
                    .HasForeignKey(d => d.IdVendedor)
                    .HasConstraintName("FK_Cliente");
            });

            modelBuilder.Entity<ClienteImpuesto>(entity =>
            {
                entity.HasKey(e => new { e.IdImpuesto, e.IdTipoImpuesto, e.IdCliente });

                entity.ToTable("Cliente_Impuesto");

                entity.HasIndex(e => e.IdCliente)
                    .HasName("IXFK_Cliente_Impuesto_Cliente");

                entity.HasIndex(e => e.IdImpuesto)
                    .HasName("IXFK_Cliente_Impuesto_Impuesto");

                entity.HasIndex(e => e.IdTipoImpuesto)
                    .HasName("IXFK_Cliente_Impuesto_TipoImpuesto");

                entity.HasOne(d => d.IdImpuestoNavigation)
                    .WithMany(p => p.ClienteImpuesto)
                    .HasForeignKey(d => d.IdImpuesto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cliente_Impuesto_Impuesto");

                entity.HasOne(d => d.IdTipoImpuestoNavigation)
                    .WithMany(p => p.ClienteImpuesto)
                    .HasForeignKey(d => d.IdTipoImpuesto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cliente_Impuesto_TipoImpuesto");
            });

            modelBuilder.Entity<ComprobanteDetalle>(entity =>
            {
                entity.ToTable("Comprobante_Detalle");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Cantidad).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.FechaMovimiento).HasColumnType("datetime");

                entity.Property(e => e.ImporteNeto).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.NumeroComprobante)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrecioNeto).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Renglon)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TipoComprobante)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdComprobanteNavigation)
                    .WithMany(p => p.ComprobanteDetalle)
                    .HasForeignKey(d => d.IdComprobante)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comprobante_Detalle_Comprobantes");

                entity.HasOne(d => d.IdProductoNavigation)
                    .WithMany(p => p.ComprobanteDetalle)
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("FK_Comprobante_Detalle_Producto");
            });

            modelBuilder.Entity<Comprobantes>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Estado)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaMovimiento).HasColumnType("datetime");

                entity.Property(e => e.ImporteGravado).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ImporteIva).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.NumeroComprobante)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TipoComprobante)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Total).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Comprobantes)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK_Comprobantes_Cliente");

                entity.HasOne(d => d.IdVendedorNavigation)
                    .WithMany(p => p.Comprobantes)
                    .HasForeignKey(d => d.IdVendedor)
                    .HasConstraintName("FK_Comprobantes_Vendedor");
            });

            modelBuilder.Entity<Familia>(entity =>
            {
                entity.HasKey(e => e.IdFamilia)
                    .HasName("PK__Familia__751F80CFFEC2F220");

                entity.Property(e => e.IdFamilia)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .HasColumnName("timestamp")
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<FamiliaFamilia>(entity =>
            {
                entity.HasKey(e => new { e.IdFamilia, e.IdFamiliaHijo })
                    .HasName("PK__Familia___ABFCC67E1F707C2E");

                entity.ToTable("Familia_Familia");

                entity.Property(e => e.IdFamilia)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.IdFamiliaHijo)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .HasColumnName("timestamp")
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.IdFamiliaNavigation)
                    .WithMany(p => p.FamiliaFamiliaIdFamiliaNavigation)
                    .HasForeignKey(d => d.IdFamilia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Familia_F__IdFam__7C4F7684");

                entity.HasOne(d => d.IdFamiliaHijoNavigation)
                    .WithMany(p => p.FamiliaFamiliaIdFamiliaHijoNavigation)
                    .HasForeignKey(d => d.IdFamiliaHijo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Familia_A__Famil__37A5467C");
            });

            modelBuilder.Entity<FamiliaPatente>(entity =>
            {
                entity.HasKey(e => new { e.IdFamilia, e.IdPatente })
                    .HasName("PK__FamiliaE__166FEEA61367E606");

                entity.ToTable("Familia_Patente");

                entity.Property(e => e.IdFamilia)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.IdPatente)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .HasColumnName("timestamp")
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.IdFamiliaNavigation)
                    .WithMany(p => p.FamiliaPatente)
                    .HasForeignKey(d => d.IdFamilia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Familia_Patente_Familia");

                entity.HasOne(d => d.IdPatenteNavigation)
                    .WithMany(p => p.FamiliaPatente)
                    .HasForeignKey(d => d.IdPatente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FamiliaElement_Patente");
            });

            modelBuilder.Entity<Impuesto>(entity =>
            {
                entity.HasIndex(e => e.IdProvincia)
                    .HasName("IXFK_Impuesto_Provincia");

                entity.HasIndex(e => e.IdTipoImpuesto)
                    .HasName("IXFK_Impuesto_TipoImpuesto");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdProvinciaNavigation)
                    .WithMany(p => p.Impuesto)
                    .HasForeignKey(d => d.IdProvincia)
                    .HasConstraintName("FK_Impuesto_Provincia");

                entity.HasOne(d => d.IdTipoImpuestoNavigation)
                    .WithMany(p => p.Impuesto)
                    .HasForeignKey(d => d.IdTipoImpuesto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Impuesto_TipoImpuesto");
            });

            modelBuilder.Entity<Patente>(entity =>
            {
                entity.HasKey(e => e.IdPatente)
                    .HasName("PK__Patente__9F4EF95C34290DD0");

                entity.Property(e => e.IdPatente)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .HasColumnName("timestamp")
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.Vista)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Presupuesto>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.FechaEmision).HasColumnType("datetime");

                entity.Property(e => e.FechaVencimiento).HasColumnType("datetime");

                entity.Property(e => e.Numero)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Presupuesto)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Presupuesto_Cliente");

                entity.HasOne(d => d.IdPresupuestoPadreNavigation)
                    .WithMany(p => p.InverseIdPresupuestoPadreNavigation)
                    .HasForeignKey(d => d.IdPresupuestoPadre)
                    .HasConstraintName("FK_Presupuesto_Presupuesto");

                entity.HasOne(d => d.IdVendedorNavigation)
                    .WithMany(p => p.Presupuesto)
                    .HasForeignKey(d => d.IdVendedor)
                    .HasConstraintName("FK_Presupuesto_Vendedor");
            });

            modelBuilder.Entity<PresupuestoDetalle>(entity =>
            {
                entity.ToTable("Presupuesto_Detalle");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Cantidad).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Descuento).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Numero)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Precio).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.IdPresupuestoNavigation)
                    .WithMany(p => p.PresupuestoDetalle)
                    .HasForeignKey(d => d.IdPresupuesto)
                    .HasConstraintName("FK_Presupuesto_Detalle_Presupuesto");

                entity.HasOne(d => d.IdProductoNavigation)
                    .WithMany(p => p.PresupuestoDetalle)
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("FK_Presupuesto_Detalle_Producto");
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaAlta).HasColumnType("datetime");
            });

            modelBuilder.Entity<Provincia>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CodigoAfip)
                    .HasColumnName("CodigoAFIP")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoImpuesto>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario)
                    .HasName("PK__Usuario__5B65BF970075BCE6");

                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Clave)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .HasColumnName("timestamp")
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.Usuario1)
                    .IsRequired()
                    .HasColumnName("Usuario")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<UsuarioFamilia>(entity =>
            {
                entity.HasKey(e => new { e.IdUsuario, e.IdFamilia })
                    .HasName("PK__Usuario___BC34479B87709AFE");

                entity.ToTable("Usuario_Familia");

                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.IdFamilia)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .HasColumnName("timestamp")
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.IdFamiliaNavigation)
                    .WithMany(p => p.UsuarioFamilia)
                    .HasForeignKey(d => d.IdFamilia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Usuario_P__Famil__35BCFE0A");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.UsuarioFamilia)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Usuario_F__IdUsu__7F2BE32F");
            });

            modelBuilder.Entity<UsuarioPatente>(entity =>
            {
                entity.HasKey(e => new { e.IdUsuario, e.IdPatente });

                entity.ToTable("Usuario_Patente");

                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.IdPatente)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .HasColumnName("timestamp")
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.IdPatenteNavigation)
                    .WithMany(p => p.UsuarioPatente)
                    .HasForeignKey(d => d.IdPatente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Patente_Patente");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.UsuarioPatente)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Patente_Usuario");
            });

            modelBuilder.Entity<Vendedor>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Direccion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
