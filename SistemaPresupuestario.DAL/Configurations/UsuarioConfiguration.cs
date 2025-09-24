using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SistemaPresupuestario.DomainModel.Seguridad;

namespace SistemaPresupuestario.DAL.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Usuario entity
    /// Defines table mapping, relationships, and constraints
    /// </summary>
    public class UsuarioConfiguration : EntityTypeConfiguration<Usuario>
    {
        public UsuarioConfiguration()
        {
            // Table mapping
            ToTable("Usuario");

            // Primary key
            HasKey(u => u.Id);
            Property(u => u.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); // We generate GUIDs in code

            // Properties
            Property(u => u.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            Property(u => u.NombreUsuario)
                .IsRequired()
                .HasMaxLength(50);

            Property(u => u.ClaveHash)
                .IsRequired()
                .HasMaxLength(500);

            Property(u => u.Salt)
                .IsRequired()
                .HasMaxLength(100);

            Property(u => u.Email)
                .HasMaxLength(255);

            Property(u => u.DebeRenovarClave)
                .IsRequired();

            Property(u => u.Activo)
                .IsRequired();

            Property(u => u.IntentosLoginFallidos)
                .IsRequired();

            Property(u => u.UltimoLogin)
                .IsOptional();

            Property(u => u.CuentaBloqueadaHasta)
                .IsOptional();

            // Concurrency control
            Property(u => u.RowVersion)
                .IsRequired()
                .IsRowVersion();

            // Timestamps
            Property(u => u.CreatedAt)
                .IsRequired();

            Property(u => u.ModifiedAt)
                .IsOptional();

            // Unique constraints
            HasIndex(u => u.NombreUsuario)
                .IsUnique()
                .HasName("IX_Usuario_NombreUsuario");

            // Many-to-many relationship with Familia
            HasMany(u => u.Familias)
                .WithMany(f => f.Usuarios)
                .Map(m =>
                {
                    m.ToTable("Usuario_Familia");
                    m.MapLeftKey("UsuarioId");
                    m.MapRightKey("FamiliaId");
                });

            // Many-to-many relationship with Patente  
            HasMany(u => u.Patentes)
                .WithMany(p => p.Usuarios)
                .Map(m =>
                {
                    m.ToTable("Usuario_Patente");
                    m.MapLeftKey("UsuarioId");
                    m.MapRightKey("PatenteId");
                });
        }
    }
}