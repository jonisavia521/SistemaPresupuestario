# Sistema Presupuestario - Nueva Arquitectura

## Arquitectura Implementada

El sistema ha sido reestructurado para eliminar la capa de Controller y utilizar una arquitectura más simple y directa para un proyecto Windows Forms universitario:

### Flujo de Datos:
```
UI (Windows Forms) → BLL → DAL → DomainModel (Entity Framework)
```

### Capas del Sistema:

#### 1. **UI Layer** (SistemaPresupuestario)
- Formularios Windows Forms (frmUsuarios, frmMain, etc.)
- Consume directamente los servicios de BLL mediante inyección de dependencias
- No tiene lógica de negocio, solo presentación

#### 2. **BLL (Business Logic Layer)**
- **DTOs**: Data Transfer Objects para transferencia de datos y validación
  - `UsuarioDTO`: Contiene validaciones y estructura de datos para UI
- **Services**: Servicios de negocio que implementan interfaces
  - `IUsuarioService` / `UsuarioService`: Maneja operaciones de usuarios
- **Mappers**: AutoMapper profiles para conversión DTO ↔ Entity
  - `UsuarioMapperProfile`: Convierte entre UsuarioDTO y Usuario (Entity)

#### 3. **DAL (Data Access Layer)**
- Context de Entity Framework (`SistemaPresupuestarioContext`)
- Manejo directo de base de datos
- Unit of Work pattern para transacciones

#### 4. **DomainModel**
- Entidades de Entity Framework generadas desde la base de datos
- Models de dominio puro

### Beneficios de esta Arquitectura:

1. **Simplicidad**: Eliminación de capa Controller innecesaria para Windows Forms
2. **Separación de Responsabilidades**: Cada capa tiene una responsabilidad específica
3. **Validación en DTOs**: Los DTOs contienen validaciones de datos usando Data Annotations
4. **Mapeo Automático**: AutoMapper maneja la conversión entre DTOs y Entities
5. **Inyección de Dependencias**: Facilita testing y mantenimiento

### Ejemplo de Uso:

```csharp
// En frmUsuarios.cs
public partial class frmUsuarios : Form
{
    private readonly IUsuarioService usuarioService;

    public frmUsuarios(IUsuarioService usuarioService)
    {
        InitializeComponent();
        this.usuarioService = usuarioService;
    }
   
    private void frmUsuarios_Load(object sender, EventArgs e)
    {
        var usuarios = usuarioService.GetAll();
        dgvUsuarios.DataSource = usuarios.ToList();
    }
}
```

### Configuración de Dependencias:

El sistema utiliza Microsoft.Extensions.DependencyInjection configurado en `Program.cs`:

```csharp
services
    .AddServicesDependencies()
    .AddBLLDependencies() // Registra AutoMapper y servicios de BLL
    .AddScoped<frmUsuarios>();
```

### Notas Importantes:

- **Controller Layer**: Completamente eliminada
- **ViewModels**: Reemplazados por DTOs en BLL
- **Mapping**: Centralizado en BLL usando AutoMapper
- **Validación**: Implementada en DTOs usando Data Annotations