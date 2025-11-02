# Actualización Vendedor - Campos Obligatorios

## Descripción
Este documento describe las actualizaciones realizadas en la entidad Vendedor para reflejar los cambios en el esquema de base de datos, donde los campos `CodigoVendedor`, `CUIT`, `PorcentajeComision`, `Activo` y `FechaAlta` ahora son **NOT NULL** (obligatorios).

## Fecha de Actualización
2024-12-XX

## Cambios en Base de Datos

### Tabla Vendedor - Cambios Aplicados

#### Campos que pasaron de NULLABLE a NOT NULL:
1. **CodigoVendedor** - `varchar(20) NOT NULL`
2. **CUIT** - `varchar(11) NOT NULL`
3. **PorcentajeComision** - `decimal(5, 2) NOT NULL` (con valor por defecto 0)
4. **Activo** - `bit NOT NULL` (con valor por defecto 1)
5. **FechaAlta** - `datetime NOT NULL` (con valor por defecto GETDATE())

### Constraints Agregados:
```sql
ALTER TABLE [dbo].[Vendedor] ADD CONSTRAINT [DF_Vendedor_ID] DEFAULT (newsequentialid()) FOR [ID]
ALTER TABLE [dbo].[Vendedor] ADD CONSTRAINT [DF__Vendedor__Porcen__09746778] DEFAULT ((0)) FOR [PorcentajeComision]
ALTER TABLE [dbo].[Vendedor] ADD CONSTRAINT [DF__Vendedor__Activo__0A688BB1] DEFAULT ((1)) FOR [Activo]
ALTER TABLE [dbo].[Vendedor] ADD CONSTRAINT [DF__Vendedor__FechaA__0B5CAFEA] DEFAULT (getdate()) FOR [FechaAlta]
```

## Archivos Modificados

### 1. Capa de Datos (DAL)

#### DAL\Implementation\EntityFramework\Vendedor.cs
**Cambios realizados:**
- Agregado `[Required]` a `CodigoVendedor`
- Agregado `[Required]` a `CUIT`
- Cambiado `PorcentajeComision` de `decimal?` a `decimal` (no nullable)
- Agregado `[Required]` a `PorcentajeComision`
- Agregado `[Required]` a `Activo`
- Agregado `[Required]` a `FechaAlta`

**Antes:**
```csharp
[StringLength(20)]
public string CodigoVendedor { get; set; }

[StringLength(11)]
public string CUIT { get; set; }

public decimal? PorcentajeComision { get; set; }

public bool Activo { get; set; }

public DateTime FechaAlta { get; set; }
```

**Después:**
```csharp
[Required]
[StringLength(20)]
public string CodigoVendedor { get; set; }

[Required]
[StringLength(11)]
public string CUIT { get; set; }

[Required]
public decimal PorcentajeComision { get; set; }

[Required]
public bool Activo { get; set; }

[Required]
public DateTime FechaAlta { get; set; }
```

#### DAL\Implementation\Repository\VendedorRepository.cs
**Cambios realizados:**
- Simplificado el método `MapearADominio()` ya que los campos ahora son NOT NULL
- Removido el operador `??` (null-coalescing) de `CodigoVendedor`, `CUIT` y `PorcentajeComision`

**Antes:**
```csharp
string codigoVendedor = vendedorEF.CodigoVendedor ?? "01";
string cuit = vendedorEF.CUIT ?? string.Empty;
decimal porcentajeComision = vendedorEF.PorcentajeComision ?? 0;
```

**Después:**
```csharp
string codigoVendedor = vendedorEF.CodigoVendedor;
string cuit = vendedorEF.CUIT;
decimal porcentajeComision = vendedorEF.PorcentajeComision;
```

### 2. Capa de Negocio (BLL)

#### BLL\DTOs\VendedorDTO.cs
**Cambios realizados:**
- Agregado `[Required]` a `PorcentajeComision`

**Antes:**
```csharp
[Range(0, 100, ErrorMessage = "La comisión debe estar entre 0 y 100")]
public decimal PorcentajeComision { get; set; }
```

**Después:**
```csharp
[Required(ErrorMessage = "El porcentaje de comisión es obligatorio")]
[Range(0, 100, ErrorMessage = "La comisión debe estar entre 0 y 100")]
public decimal PorcentajeComision { get; set; }
```

### 3. Capa de Dominio (DomainModel)

**DomainModel\Domain\VendedorDM.cs**
- ? No requirió cambios
- La lógica de validación ya contemplaba estos campos como obligatorios

## Impacto en la Aplicación

### ? Validaciones Mejoradas
1. **A nivel de Base de Datos**: Los constraints NOT NULL aseguran integridad de datos
2. **A nivel de EF**: Los atributos `[Required]` generan validaciones automáticas
3. **A nivel de DTO**: DataAnnotations validan en la UI antes de enviar a la BLL
4. **A nivel de Dominio**: Validaciones de negocio se mantienen intactas

### ? Comportamiento por Defecto
- **PorcentajeComision**: Se inicializa en 0 si no se especifica
- **Activo**: Se inicializa en `true` (1) automáticamente
- **FechaAlta**: Se establece automáticamente con la fecha/hora actual

### ?? Consideraciones Importantes

1. **Migración de Datos Existentes**
   - Si hay registros con valores NULL en estos campos, el script SQL fallará
   - Se debe ejecutar un UPDATE previo para establecer valores por defecto

2. **Formularios de UI**
   - El formulario de alta/edición ya tenía estos campos como obligatorios
   - No requiere cambios en la UI

3. **Compatibilidad hacia Atrás**
   - El código anterior que no pasaba estos valores ahora fallará
   - Se recomienda revisar cualquier proceso de importación de datos

## Script de Migración de Datos

Si existen registros con valores NULL, ejecutar ANTES del ALTER TABLE:

```sql
-- Actualizar registros existentes con valores NULL
UPDATE [dbo].[Vendedor]
SET 
    CodigoVendedor = ISNULL(CodigoVendedor, '01'),
    CUIT = ISNULL(CUIT, '00000000000'),
    PorcentajeComision = ISNULL(PorcentajeComision, 0),
    Activo = ISNULL(Activo, 1),
    FechaAlta = ISNULL(FechaAlta, GETDATE())
WHERE 
    CodigoVendedor IS NULL 
    OR CUIT IS NULL 
    OR PorcentajeComision IS NULL 
    OR Activo IS NULL 
    OR FechaAlta IS NULL;
GO
```

## Verificación de Cambios

### ? Compilación Exitosa
El proyecto compila sin errores después de los cambios.

### ? Validaciones Aplicadas
```csharp
// Al crear un vendedor, ahora es obligatorio pasar todos estos valores
var vendedor = new VendedorDM(
    codigoVendedor: "01",      // OBLIGATORIO
    nombre: "Juan Pérez",      // OBLIGATORIO
    cuit: "20123456789",       // OBLIGATORIO
    porcentajeComision: 5.5m   // OBLIGATORIO
);
```

### ? Manejo en Repositorio
El mapeo entre entidades ahora es más limpio al no necesitar verificaciones de null para estos campos.

## Testing Recomendado

1. **Prueba de Alta**
   - ? Crear vendedor con todos los campos obligatorios
   - ? Intentar crear vendedor sin CodigoVendedor (debe fallar)
   - ? Intentar crear vendedor sin CUIT (debe fallar)
   - ? Intentar crear vendedor sin PorcentajeComision (debe usar 0 por defecto)

2. **Prueba de Edición**
   - ? Editar vendedor existente
   - ? Verificar que no se pueden borrar campos obligatorios

3. **Prueba de Base de Datos**
   - ? Ejecutar el script CREATE TABLE en una BD limpia
   - ? Verificar que los defaults funcionan correctamente
   - ? Intentar insertar un registro sin los campos obligatorios (debe fallar)

## Próximos Pasos

1. **Ejecutar Script SQL**: Aplicar el nuevo CREATE TABLE en todos los ambientes
2. **Migrar Datos**: Si existen datos, ejecutar el script de migración primero
3. **Desplegar Código**: Actualizar la aplicación con los cambios en EF
4. **Verificar Funcionalidad**: Probar el ABM completo de Vendedores

## Conclusión

Los cambios aseguran mayor integridad de datos al nivel de base de datos y mejoran las validaciones en todas las capas de la aplicación. El código es más robusto y predecible al eliminar la ambigüedad de valores nulos en campos críticos.

---

**Estado**: ? Completado
**Compilación**: ? Exitosa
**Fecha**: 2024-12-XX
**Versión**: 1.1
