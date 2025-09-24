using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SistemaPresupuestario.DomainModel.Seguridad;

namespace SistemaPresupuestario.DAL.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Familia entity
    /// Defines table mapping, self-referencing relationships, and constraints
    /// </summary>
    public class FamiliaConfiguration : EntityTypeConfiguration<Familia>
    {
        public FamiliaConfiguration()
        {
            // Table mapping
            ToTable("Familia");

            // Primary key
            HasKey(f => f.Id);
            Property(f => f.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); // We generate GUIDs in code

            // Properties
            Property(f => f.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            Property(f => f.Descripcion)
                .HasMaxLength(500);

            Property(f => f.FamiliaPadreId)
                .IsOptional();

            // Concurrency control
            Property(f => f.RowVersion)
                .IsRequired()
                .IsRowVersion();

            // Timestamps
            Property(f => f.CreatedAt)
                .IsRequired();

            Property(f => f.ModifiedAt)
                .IsOptional();

            // Self-referencing relationship (hierarchical structure)
            HasOptional(f => f.FamiliaPadre)
                .WithMany(f => f.FamiliasHijas)
                .HasForeignKey(f => f.FamiliaPadreId)
                .WillCascadeOnDelete(false); // Prevent cascade delete to avoid cycles

            // Many-to-many relationship with Patente
            HasMany(f => f.Patentes)
                .WithMany(p => p.Familias)
                .Map(m =>
                {
                    m.ToTable("Familia_Patente");
                    m.MapLeftKey("FamiliaId");
                    m.MapRightKey("PatenteId");
                });

            // Many-to-many relationship with Usuario (configured in UsuarioConfiguration)
        }
    }
}