using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SistemaPresupuestario.DAL.Configurations;
using SistemaPresupuestario.DomainModel.Seguridad;

namespace SistemaPresupuestario.DAL.Context
{
    /// <summary>
    /// Entity Framework DbContext for Sistema Presupuestario
    /// Configures all entity mappings and database relationships
    /// </summary>
    public class SistemaPresupuestarioContext : DbContext
    {
        /// <summary>
        /// Users in the system
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }

        /// <summary>
        /// Permission families (composite pattern containers)
        /// </summary>
        public DbSet<Familia> Familias { get; set; }

        /// <summary>
        /// Individual permissions/patents (composite pattern leaves)
        /// </summary>
        public DbSet<Patente> Patentes { get; set; }

        /// <summary>
        /// Constructor using default connection string
        /// </summary>
        public SistemaPresupuestarioContext() : base("name=SistemaPresupuestarioContext")
        {
            // Enable lazy loading
            Configuration.LazyLoadingEnabled = true;
            
            // Disable proxy creation to avoid circular reference issues in serialization
            Configuration.ProxyCreationEnabled = false;
            
            // Validate on save
            Configuration.ValidateOnSaveEnabled = true;
            
            // Use database initializer for development
            Database.SetInitializer(new SistemaPresupuestarioDbInitializer());
        }

        /// <summary>
        /// Constructor with connection string
        /// </summary>
        public SistemaPresupuestarioContext(string connectionString) : base(connectionString)
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = true;
            Database.SetInitializer(new SistemaPresupuestarioDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove pluralizing table name convention
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Add entity configurations
            modelBuilder.Configurations.Add(new UsuarioConfiguration());
            modelBuilder.Configurations.Add(new FamiliaConfiguration());
            modelBuilder.Configurations.Add(new PatenteConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Override SaveChanges to update ModifiedAt timestamp
        /// </summary>
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<DomainModel.Seguridad.Base.EntityBase>();
            
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAt = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }

    /// <summary>
    /// Database initializer for development environment
    /// Creates database and seeds initial data
    /// </summary>
    public class SistemaPresupuestarioDbInitializer : CreateDatabaseIfNotExists<SistemaPresupuestarioContext>
    {
        protected override void Seed(SistemaPresupuestarioContext context)
        {
            // Create sample patents
            var patenteUsuarios = new Patente("Gestión de Usuarios", "FrmUsuarios", "Permite gestionar usuarios del sistema");
            var patenteReportes = new Patente("Reportes", "FrmReportes", "Permite generar reportes");
            var patenteConfiguracion = new Patente("Configuración", "FrmConfiguracion", "Permite acceder a configuración del sistema");
            var patenteLecturaUsuarios = new Patente("Lectura de Usuarios", "FrmUsuarios", "Solo permite ver usuarios");
            var patenteEdicionUsuarios = new Patente("Edición de Usuarios", "FrmUsuarioEdit", "Permite editar usuarios");

            context.Patentes.Add(patenteUsuarios);
            context.Patentes.Add(patenteReportes);
            context.Patentes.Add(patenteConfiguracion);
            context.Patentes.Add(patenteLecturaUsuarios);
            context.Patentes.Add(patenteEdicionUsuarios);

            // Create sample families (hierarchical structure)
            var familiaLectores = new Familia("Lectores", "Usuarios con permisos básicos de lectura");
            familiaLectores.Patentes.Add(patenteLecturaUsuarios);

            var familiaEditores = new Familia("Editores", "Usuarios con permisos de edición");
            familiaEditores.Patentes.Add(patenteEdicionUsuarios);
            familiaEditores.FamiliasHijas.Add(familiaLectores);
            familiaLectores.FamiliaPadre = familiaEditores;
            familiaLectores.FamiliaPadreId = familiaEditores.Id;

            var familiaAdministradores = new Familia("Administradores", "Usuarios con todos los permisos");
            familiaAdministradores.Patentes.Add(patenteUsuarios);
            familiaAdministradores.Patentes.Add(patenteReportes);
            familiaAdministradores.Patentes.Add(patenteConfiguracion);
            familiaAdministradores.FamiliasHijas.Add(familiaEditores);
            familiaEditores.FamiliaPadre = familiaAdministradores;
            familiaEditores.FamiliaPadreId = familiaAdministradores.Id;

            context.Familias.Add(familiaLectores);
            context.Familias.Add(familiaEditores);
            context.Familias.Add(familiaAdministradores);

            // Create sample users
            var adminUser = new Usuario("Administrador", "admin", 
                "AQAAAAEAACcQAAAAEJ4VQ1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q=", // Hashed "admin123"
                "salt123");
            adminUser.Familias.Add(familiaAdministradores);
            adminUser.Email = "admin@sistema.com";

            var editorUser = new Usuario("Editor", "editor",
                "AQAAAAEAACcQAAAAEJ4VQ2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q2Q=", // Hashed "editor123"
                "salt456");
            editorUser.Familias.Add(familiaEditores);
            editorUser.Email = "editor@sistema.com";

            var lectorUser = new Usuario("Lector", "lector",
                "AQAAAAEAACcQAAAAEJ4VQ3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q3Q=", // Hashed "lector123"
                "salt789");
            lectorUser.Familias.Add(familiaLectores);
            lectorUser.Email = "lector@sistema.com";

            context.Usuarios.Add(adminUser);
            context.Usuarios.Add(editorUser);
            context.Usuarios.Add(lectorUser);

            context.SaveChanges();
            base.Seed(context);
        }
    }
}