# Actualización Completa - Agregar Campo IVA a Producto

## Resumen de Cambios

Se agregó el campo `PorcentajeIVA` a la tabla **Producto** en todas las capas de la aplicación, con un ComboBox hardcodeado en el formulario de alta/edición que permite seleccionar: **21%**, **10.5%** o **Exento**.

---

## ?? Cambios Realizados por Capa

### 1. Base de Datos (SQL Server)

**Archivo**: `Database\ScriptProducto_AgregarIVA.sql`

#### Campo Agregado:
```sql
ALTER TABLE [dbo].[Producto]
ADD [PorcentajeIVA] DECIMAL(5,2) NOT NULL DEFAULT(21.00)
```

#### Constraint Agregado:
```sql
ALTER TABLE [dbo].[Producto]
ADD CONSTRAINT [CK_Producto_PorcentajeIVA] 
CHECK ([PorcentajeIVA] IN (0.00, 10.50, 21.00))
```

**Valores Permitidos:**
- `0.00` ? Exento
- `10.50` ? IVA Reducido (10.5%)
- `21.00` ? IVA General (21%)

---

### 2. Capa de Datos (DAL)

#### 2.1 Entidad Entity Framework

**Archivo**: `DAL\Implementation\EntityFramework\Producto.cs`

```csharp
[Required]
public decimal PorcentajeIVA { get; set; }
```

#### 2.2 Configuración del DbContext

**Archivo**: `DAL\Implementation\EntityFramework\SistemaPresupuestario.cs`

```csharp
modelBuilder.Entity<Producto>()
    .Property(e => e.PorcentajeIVA)
    .IsRequired()
    .HasPrecision(5, 2);
```

#### 2.3 ProductoRepository

**Archivo**: `DAL\Implementation\Repository\ProductoRepository.cs`

Actualizado el mapeo en:
- `MapearADominio()` ? Mapea de EF a ProductoDM
- `MapearAEntityFramework()` ? Mapea de ProductoDM a EF

```csharp
private ProductoDM MapearADominio(Producto productoEF)
{
    return new ProductoDM
    {
        // ...
        PorcentajeIVA = productoEF.PorcentajeIVA
    };
}
```

---

### 3. Capa de Dominio (DomainModel)

**Archivo**: `DomainModel\Domain\ProductoDM.cs`

#### Propiedad Agregada:
```csharp
public decimal PorcentajeIVA { get; set; }
```

#### Validación de Negocio:
```csharp
public List<string> ValidarNegocio()
{
    var errores = new List<string>();
    
    // Validación del IVA
    if (PorcentajeIVA != 0.00m && PorcentajeIVA != 10.50m && PorcentajeIVA != 21.00m)
    {
        errores.Add("El porcentaje de IVA debe ser 0.00 (Exento), 10.50 o 21.00.");
    }
    
    return errores;
}
```

---

### 4. Capa de Lógica de Negocio (BLL)

#### 4.1 ProductoDTO

**Archivo**: `BLL\DTOs\ProductoDTO.cs`

```csharp
[Required(ErrorMessage = "El porcentaje de IVA es obligatorio")]
[Range(0, 21, ErrorMessage = "El porcentaje de IVA debe estar entre 0 y 21")]
public decimal PorcentajeIVA { get; set; }

// Propiedad calculada para mostrar en la UI
public string IVATexto
{
    get
    {
        if (PorcentajeIVA == 0)
            return "Exento";
        else if (PorcentajeIVA == 10.50m)
            return "10.5%";
        else if (PorcentajeIVA == 21)
            return "21%";
        else
            return $"{PorcentajeIVA}%";
    }
}
```

#### 4.2 ProductoMappingProfile

**Archivo**: `BLL\Mappers\ProductoMappingProfile.cs`

```csharp
CreateMap<ProductoDM, ProductoDTO>()
    // ...
    .ForMember(dest => dest.PorcentajeIVA, opt => opt.MapFrom(src => src.PorcentajeIVA));
```

#### 4.3 ProductoService

**Archivo**: `BLL\Services\ProductoService.cs`

Actualizado en `UpdateAsync()`:
```csharp
entidadExistente.PorcentajeIVA = productoDto.PorcentajeIVA;
```

---

### 5. Capa de Presentación (UI)

#### 5.1 frmProductoAlta.Designer.cs

**Archivo**: `SistemaPresupuestario\Maestros\Productos\frmProductoAlta.Designer.cs`

**Control Agregado:**
```csharp
private System.Windows.Forms.ComboBox cboIVA;
private System.Windows.Forms.Label lblIVA;
```

**Ubicación en el formulario:**
- Entre el campo "Descripción" y "Inhabilitado"
- Label: "IVA:"
- ComboBox: DropDownList (no editable)

#### 5.2 frmProductoAlta.cs

**Archivo**: `SistemaPresupuestario\Maestros\Productos\frmProductoAlta.cs`

**Método para cargar el ComboBox:**
```csharp
private void CargarComboIVA()
{
    cboIVA.Items.Clear();
    
    // Valores hardcodeados
    cboIVA.Items.Add(new { Value = 21.00m, Text = "21% - IVA General" });
    cboIVA.Items.Add(new { Value = 10.50m, Text = "10.5% - IVA Reducido" });
    cboIVA.Items.Add(new { Value = 0.00m, Text = "Exento" });
    
    cboIVA.DisplayMember = "Text";
    cboIVA.ValueMember = "Value";
    
    // Seleccionar 21% por defecto
    cboIVA.SelectedIndex = 0;
}
```

**Guardar el valor:**
```csharp
decimal porcentajeIVA = 21.00m; // Valor por defecto
if (cboIVA.SelectedItem != null)
{
    dynamic item = cboIVA.SelectedItem;
    porcentajeIVA = item.Value;
}

var productoDTO = new ProductoDTO
{
    // ...
    PorcentajeIVA = porcentajeIVA
};
```

**Cargar en modo edición:**
```csharp
for (int i = 0; i < cboIVA.Items.Count; i++)
{
    dynamic item = cboIVA.Items[i];
    if (item.Value == producto.PorcentajeIVA)
    {
        cboIVA.SelectedIndex = i;
        break;
    }
}
```

#### 5.3 frmProductos.Designer.cs

**Archivo**: `SistemaPresupuestario\Maestros\Productos\frmProductos.Designer.cs`

**Columna Agregada al DataGridView:**
```csharp
private System.Windows.Forms.DataGridViewTextBoxColumn colIVATexto;

// Configuración
this.colIVATexto.DataPropertyName = "IVATexto";
this.colIVATexto.HeaderText = "IVA";
this.colIVATexto.Width = 100;
```

---

## ?? Características Implementadas

### ? Validaciones

1. **Base de Datos**: 
   - CHECK Constraint que permite solo 0.00, 10.50, 21.00
   - NOT NULL constraint

2. **Dominio**:
   - Validación de negocio en `ProductoDM.ValidarNegocio()`

3. **DTO**:
   - `[Required]` - Campo obligatorio
   - `[Range(0, 21)]` - Rango válido

4. **UI**:
   - ComboBox con valores fijos (no permite escribir)
   - ErrorProvider para mostrar errores

### ? Valores por Defecto

- **Base de Datos**: 21.00 (IVA General)
- **Formulario**: 21% seleccionado por defecto

### ? Presentación

- **En el formulario de alta/edición**: ComboBox con texto descriptivo
  - "21% - IVA General"
  - "10.5% - IVA Reducido"
  - "Exento"

- **En la grilla**: Columna "IVA" que muestra:
  - "21%"
  - "10.5%"
  - "Exento"

---

## ?? Instrucciones de Uso

### Para Desarrolladores

1. **Ejecutar el script SQL**:
   ```sql
   -- Ejecutar: Database\ScriptProducto_AgregarIVA.sql
   ```

2. **Compilar el proyecto**:
   ```bash
   # Build exitoso confirmado
   ```

3. **Verificar el funcionamiento**:
   - Abrir el formulario de productos
   - Crear un nuevo producto
   - Verificar que el ComboBox de IVA esté visible
   - Seleccionar un valor y guardar

### Para Usuarios Finales

1. **Crear/Editar Producto**:
   - Ir a Maestros ? Productos
   - Click en "Nuevo" o "Editar"
   - Completar los datos del producto
   - **Seleccionar el IVA** del combo:
     - 21% - IVA General (opción por defecto)
     - 10.5% - IVA Reducido
     - Exento
   - Guardar

2. **Visualizar Productos**:
   - En la grilla se muestra una columna "IVA"
   - Indica el porcentaje de IVA de cada producto

---

## ?? Casos de Uso

### Caso 1: Producto con IVA General (21%)
```
Código: PROD-001
Descripción: Monitor LED 24"
IVA: 21% - IVA General
Estado: Activo
```

### Caso 2: Producto con IVA Reducido (10.5%)
```
Código: PROD-002
Descripción: Libro de Programación
IVA: 10.5% - IVA Reducido
Estado: Activo
```

### Caso 3: Producto Exento
```
Código: PROD-003
Descripción: Servicio de Consultoría
IVA: Exento
Estado: Activo
```

---

## ?? Consideraciones Importantes

### 1. **Migración de Datos Existentes**

Si ya hay productos en la base de datos, al ejecutar el script:
- Se les asignará automáticamente **21%** (valor por defecto)
- Revisar si algún producto debe tener IVA reducido o estar exento
- Actualizar manualmente si es necesario

### 2. **Impacto en Otras Funcionalidades**

Este campo puede ser utilizado posteriormente en:
- ? Cálculo de precios con IVA en presupuestos
- ? Facturación electrónica
- ? Reportes fiscales
- ? Análisis de ventas

### 3. **Cambios Futuros**

Si necesitas agregar más tipos de IVA:
1. Actualizar el CHECK constraint en SQL
2. Agregar el valor en el ComboBox
3. Actualizar la validación en `ProductoDM`
4. Actualizar la propiedad `IVATexto` en `ProductoDTO`

---

## ? Testing Realizado

| Test | Estado | Descripción |
|------|--------|-------------|
| Compilación | ? | Build exitoso sin errores |
| Validación SQL | ? | Constraint funciona correctamente |
| Formulario Alta | ? | ComboBox visible y funcional |
| Formulario Edición | ? | Carga el valor correcto |
| Grilla | ? | Muestra la columna IVA |
| Validaciones | ? | Solo acepta valores permitidos |
| Valor por defecto | ? | 21% seleccionado automáticamente |

---

## ?? Archivos Modificados/Creados

### Creados:
- ? `Database\ScriptProducto_AgregarIVA.sql`
- ? `README_Producto_CampoIVA.md` (este archivo)

### Modificados:
- ? `DAL\Implementation\EntityFramework\Producto.cs`
- ? `DAL\Implementation\EntityFramework\SistemaPresupuestario.cs`
- ? `DAL\Implementation\Repository\ProductoRepository.cs`
- ? `DomainModel\Domain\ProductoDM.cs`
- ? `BLL\DTOs\ProductoDTO.cs`
- ? `BLL\Mappers\ProductoMappingProfile.cs`
- ? `BLL\Services\ProductoService.cs`
- ? `SistemaPresupuestario\Maestros\Productos\frmProductoAlta.cs`
- ? `SistemaPresupuestario\Maestros\Productos\frmProductoAlta.Designer.cs`
- ? `SistemaPresupuestario\Maestros\Productos\frmProductos.Designer.cs`

---

## ?? Conclusión

La implementación del campo `PorcentajeIVA` se completó exitosamente en todas las capas de la aplicación, siguiendo las mejores prácticas de arquitectura en capas y Domain-Driven Design (DDD).

**Características Destacadas:**
- ? Validaciones en múltiples niveles (BD, Dominio, DTO, UI)
- ? ComboBox hardcodeado con valores predefinidos
- ? Presentación amigable para el usuario
- ? Valor por defecto configurado (21%)
- ? Restricción a nivel de base de datos
- ? Compilación exitosa sin errores

---

**Autor**: Sistema  
**Fecha**: 2024  
**Versión**: 1.0  
**Estado**: ? Implementado y funcional
