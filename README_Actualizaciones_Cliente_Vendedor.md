# Guía de Implementación - Actualización Cliente y ABM Vendedores

## ? Cambios Realizados en Cliente

### 1. **Campo Localidad Agregado**
- ? ClienteDM: Agregado campo `Localidad` (opcional)
- ? ClienteDTO: Agregado campo `Localidad`
- ? ClienteService: Incluye Localidad en Add y Update
- ? ClienteMappingProfile: Mapeo de Localidad
- ? ClienteRepository: Mapeo bidireccional con Localidad
- ? frmClienteAlta: Control TextBox para Localidad agregado

### 2. **TipoDocumento Separado del CUIT**
**Antes**: El tipo de documento se concatenaba con el CUIT como `"CUIT:20123456789"`

**Ahora**: Campos separados en la base de datos:
- `TipoDocumento` (VARCHAR(10)): DNI | CUIT | CUIL
- `CUIT` (VARCHAR(50)): Solo el número del documento

? **Cambios implementados**:
- Cliente.cs (EF): Agregado campo `TipoDocumento`
- SistemaPresupuestario.cs (DbContext): Configuración Fluent API
- ClienteRepository: Mapeo actualizado para usar campos separados
- Script SQL: Migración automática de datos existentes

## ?? ABM Vendedores - ARCHIVOS POR CREAR

### Estado Actual
? **Completado**:
1. VendedorDM.cs (Entidad de dominio)
2. Vendedor.cs (Entidad EF actualizada con nuevos campos)

?? **Pendiente de creación** (siguiendo el mismo patrón de Cliente):

### 1. DomainModel/Contract/IVendedorRepository.cs
```csharp
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainModel.Contract
{
    public interface IVendedorRepository : IRepository<VendedorDM>
    {
        Task<VendedorDM> GetByCodigoAsync(string codigoVendedor);
        Task<VendedorDM> GetByCUITAsync(string cuit);
        Task<IEnumerable<VendedorDM>> GetActivosAsync();
        Task<bool> ExisteCodigoAsync(string codigoVendedor, Guid? excluyendoId = null);
        Task<bool> ExisteCUITAsync(string cuit, Guid? excluyendoId = null);
    }
}
```

### 2. BLL/DTOs/VendedorDTO.cs
```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class VendedorDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El código de vendedor es obligatorio")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "El código debe tener exactamente 2 dígitos")]
        public string CodigoVendedor { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(200, MinimumLength = 3)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El CUIT es obligatorio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El CUIT debe tener 11 dígitos")]
        public string CUIT { get; set; }

        [Range(0, 100, ErrorMessage = "La comisión debe estar entre 0 y 100")]
        public decimal PorcentajeComision { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public string EstadoTexto => Activo ? "Activo" : "Inactivo";
    }
}
```

### 3. BLL/Contracts/IVendedorService.cs
```csharp
using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Contracts
{
    public interface IVendedorService
    {
        Task<IEnumerable<VendedorDTO>> GetAllAsync();
        Task<IEnumerable<VendedorDTO>> GetActivosAsync();
        Task<VendedorDTO> GetByIdAsync(Guid id);
        Task<VendedorDTO> GetByCodigoAsync(string codigoVendedor);
        Task<VendedorDTO> GetByCUITAsync(string cuit);
        Task<bool> AddAsync(VendedorDTO vendedorDTO);
        Task<bool> UpdateAsync(VendedorDTO vendedorDTO);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ReactivarAsync(Guid id);
    }
}
```

### 4. BLL/Services/VendedorService.cs
(Implementación similar a ClienteService.cs)

### 5. BLL/Mappers/VendedorMappingProfile.cs
```csharp
using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    public class VendedorMappingProfile : Profile
    {
        public VendedorMappingProfile()
        {
            CreateMap<VendedorDM, VendedorDTO>();
            CreateMap<VendedorDTO, VendedorDM>();
        }
    }
}
```

### 6. DAL/Implementation/Repository/VendedorRepository.cs
(Implementación similar a ClienteRepository.cs con mapeos bidireccionales)

### 7. UI - Formularios
- **SistemaPresupuestario/Maestros/Vendedores/frmVendedores.cs** (Listado)
- **SistemaPresupuestario/Maestros/Vendedores/frmVendedores.Designer.cs**
- **SistemaPresupuestario/Maestros/Vendedores/frmVendedorAlta.cs** (Alta/Edición)
- **SistemaPresupuestario/Maestros/Vendedores/frmVendedorAlta.Designer.cs**

### 8. Registros de Dependencias

**BLL/DependencyContainer.cs**:
```csharp
services.AddAutoMapper(typeof(VendedorMappingProfile));
services.AddScoped<IVendedorService, VendedorService>();
```

**DAL/DependencyContainer.cs**:
```csharp
services.AddScoped<IVendedorRepository, VendedorRepository>();
```

**SistemaPresupuestario/Program.cs**:
```csharp
services
    .AddTransient<frmVendedores>()
    .AddTransient<frmVendedorAlta>();
```

### 9. Menú Principal
Agregar en frmMain.cs:
```csharp
private void tsVendedor_Click(object sender, EventArgs e)
{
    var formAbierto = Application.OpenForms.OfType<frmVendedores>()
        .FirstOrDefault(f => !f.IsDisposed);

    if (formAbierto != null)
    {
        formAbierto.BringToFront();
    }
    else
    {
        var hijo = _serviceProvider.GetService(typeof(frmVendedores)) as frmVendedores;
        hijo.MdiParent = this;
        hijo.Show();
    }
}
```

## ?? Script SQL para Vendedores

```sql
USE [SistemaPresupuestario]
GO

-- Agregar nuevos campos a la tabla Vendedor
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'CodigoVendedor')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [CodigoVendedor] VARCHAR(20) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'CUIT')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [CUIT] VARCHAR(11) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'PorcentajeComision')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [PorcentajeComision] DECIMAL(5,2) NULL DEFAULT 0
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'Activo')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [Activo] BIT NOT NULL DEFAULT 1
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'FechaAlta')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [FechaAlta] DATETIME NOT NULL DEFAULT GETDATE()
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'FechaModificacion')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [FechaModificacion] DATETIME NULL
END
GO

PRINT 'Script ejecutado exitosamente. Campos agregados a la tabla Vendedor.'
GO
```

## ?? Campos del Formulario frmVendedorAlta

### Grupo: Datos Básicos
- Código Vendedor (2 dígitos, obligatorio, no editable)
- Nombre (obligatorio)
- CUIT (11 dígitos, obligatorio, con validación)

### Grupo: Datos Comerciales
- Porcentaje Comisión (0-100, obligatorio)

### Grupo: Datos de Contacto (Opcional)
- Email
- Teléfono
- Dirección

## ?? Flujo de Trabajo Recomendado

1. ? Ejecutar script SQL para Cliente (ya creado)
2. ?? Crear archivos de Vendedor en orden:
   - IVendedorRepository
   - VendedorDTO
   - IVendedorService
   - VendedorService
   - VendedorMappingProfile
   - VendedorRepository
   - frmVendedores (listado)
   - frmVendedorAlta (alta/edición)
3. ?? Registrar dependencias en IoC
4. ?? Agregar opción en menú principal
5. ?? Ejecutar script SQL para Vendedor
6. ?? Compilar y probar

## ?? Notas Importantes

### Cliente - Cambios Completados
- ? Localidad es opcional y se muestra en el formulario
- ? TipoDocumento ahora es un campo separado en BD
- ? El CUIT almacena solo el número (sin tipo)
- ? Migración automática de datos existentes en el script SQL

### Vendedor - A Implementar
- El código de vendedor se genera automáticamente (VEN-01, VEN-02, etc.)
- El CUIT es obligatorio y debe ser válido
- La comisión por defecto es 0%
- Todos los vendedores inician como Activos
- La eliminación es lógica (campo Activo)

## ?? Advertencias

1. **No ejecutar** los scripts SQL sin antes hacer backup de la BD
2. **Revisar** que no haya datos que dependan de la estructura antigua de CUIT
3. **Probar** primero en ambiente de desarrollo
4. Los formularios de Vendedor deben crearse siguiendo **exactamente** el mismo patrón que Cliente para mantener consistencia

## ?? Próximos Pasos

Para completar el ABM de Vendedores, necesitas:
1. Crear los 9 archivos pendientes listados arriba
2. Seguir el mismo patrón de Cliente (copiar y adaptar)
3. Registrar las dependencias en los 3 contenedores
4. Agregar la opción de menú
5. Ejecutar el script SQL
6. Compilar y probar

¿Deseas que continúe creando los archivos restantes del ABM de Vendedores?
