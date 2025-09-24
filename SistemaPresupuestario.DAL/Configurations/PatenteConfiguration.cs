using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SistemaPresupuestario.DomainModel.Seguridad;

namespace SistemaPresupuestario.DAL.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Patente entity
    /// Defines table mapping, relationships, and constraints
    /// </summary>
    public class PatenteConfiguration : EntityTypeConfiguration<Patente>
    {
        public PatenteConfiguration()
        {
            // Table mapping
            ToTable("Patente");

            // Primary key
            HasKey(p => p.Id);
            Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); // We generate GUIDs in code

            // Properties
            Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            Property(p => p.Vista)
                .HasMaxLength(200);

            Property(p => p.Descripcion)
                .HasMaxLength(500);

            // Concurrency control
            Property(p => p.RowVersion)
                .IsRequired()
                .IsRowVersion();

            // Timestamps
            Property(p => p.CreatedAt)
                .IsRequired();

            Property(p => p.ModifiedAt)
                .IsOptional();

            // Many-to-many relationships are configured in Usuario and Familia configurations
        }
    }
}