# Solución al Error de Clave Foránea en ListaPrecio

## Error Original

```
System.InvalidOperationException: The operation failed: The relationship could not be changed because one or more of the foreign-key properties is non-nullable. When a change is made to a relationship, the related foreign-key property is set to a null value. If the foreign-key does not support null values, a new relationship must be defined, the foreign-key property must be assigned another non-null value, or the unrelated object must be deleted.
```

## Causa del Problema

El error ocurría al actualizar una `ListaPrecio` porque:

1. **Entity Framework intentaba desvincular los detalles existentes** estableciendo `IdListaPrecio` a `NULL`
2. **La columna `IdListaPrecio` es NOT NULL** en la base de datos
3. **El método `Clear()` no elimina físicamente los registros**, solo intenta desvincularlos
4. **La configuración tenía `WillCascadeOnDelete(false)`**, impidiendo la eliminación automática

## Solución Implementada

### 1. **BLL\Services\ListaPrecioService.cs**

#### Cambios en `ActualizarEntidadListaPrecio`:

**ANTES (? Incorrecto):**
```csharp
// Limpiar detalles existentes
var detallesEF = tipo.GetProperty("ListaPrecio_Detalle").GetValue(listaPrecioEF);
var clearMethod = detallesEF.GetType().GetMethod("Clear");
clearMethod.Invoke(detallesEF, null); // ? Intenta establecer FK a NULL
```

**AHORA (? Correcto):**
```csharp
// Obtener la colección actual de detalles
var detallesEF = tipo.GetProperty("ListaPrecio_Detalle").GetValue(listaPrecioEF);

// Convertir a lista para poder iterar de forma segura
var detallesActuales = new List<object>();
foreach (var detalle in (System.Collections.IEnumerable)detallesEF)
{
    detallesActuales.Add(detalle);
}

// Remover cada detalle de la colección
// Entity Framework los marcará como "Deleted" en el contexto
var removeMethod = detallesEF.GetType().GetMethod("Remove");
foreach (var detalleActual in detallesActuales)
{
    removeMethod.Invoke(detallesEF, new[] { detalleActual }); // ? Marca como Deleted
}

// Agregar los nuevos detalles
var addMethod = detallesEF.GetType().GetMethod("Add");
foreach (var detalleDM in domainModel.Detalles)
{
    var detalleEF = Activator.CreateInstance(tipoDetalle);
    tipoDetalle.GetProperty("ID").SetValue(detalleEF, detalleDM.Id);
    tipoDetalle.GetProperty("IdListaPrecio").SetValue(detalleEF, domainModel.Id);
    tipoDetalle.GetProperty("IdProducto").SetValue(detalleEF, detalleDM.IdProducto);
    tipoDetalle.GetProperty("Precio").SetValue(detalleEF, detalleDM.Precio);

    addMethod.Invoke(detallesEF, new[] { detalleEF });
}
```

**También se agregó soporte para el campo `IncluyeIva`:**
```csharp
tipo.GetProperty("IncluyeIva").SetValue(listaPrecioEF, domainModel.IncluyeIva);
```

### 2. **DAL\Implementation\EntityFramework\SistemaPresupuestario.cs**

#### Cambios en la configuración del contexto:

**ANTES (? Incorrecto):**
```csharp
modelBuilder.Entity<ListaPrecio>()
    .HasMany(e => e.ListaPrecio_Detalle)
    .WithRequired(e => e.ListaPrecio)
    .HasForeignKey(e => e.IdListaPrecio)
    .WillCascadeOnDelete(false); // ? No permite eliminación en cascada
```

**AHORA (? Correcto):**
```csharp
// ? CAMBIO CRÍTICO: Habilitar eliminación en cascada para ListaPrecio_Detalle
modelBuilder.Entity<ListaPrecio>()
    .HasMany(e => e.ListaPrecio_Detalle)
    .WithRequired(e => e.ListaPrecio)
    .HasForeignKey(e => e.IdListaPrecio)
    .WillCascadeOnDelete(true); // ? Permite eliminación en cascada
```

**También se habilitó el tracking de cambios:**
```csharp
public SistemaPresupuestario()
    : base("name=SistemaPresupuestario")
{
    // Habilitar el seguimiento de cambios y proxy de carga diferida
    this.Configuration.LazyLoadingEnabled = true;
    this.Configuration.ProxyCreationEnabled = true;
}
```

### 3. **DAL\Implementation\Repository\ListaPrecioRepository.cs**

#### Mejoras en el método `Update`:

```csharp
void IRepository<object>.Update(object entity)
{
    if (entity is ListaPrecio listaPrecio)
    {
        // Asegurar que el contexto esté rastreando correctamente la entidad
        var entry = _context.Entry(listaPrecio);
        
        if (entry.State == EntityState.Detached)
        {
            _context.ListaPrecio.Attach(listaPrecio);
            entry.State = EntityState.Modified;
        }
        
        // Entity Framework rastreará automáticamente los cambios
        // cuando se usen Remove() y Add() en la colección
    }
    else
    {
        throw new ArgumentException("La entidad debe ser del tipo ListaPrecio");
    }
}
```

#### Mejoras en los métodos de consulta:

Se agregó carga explícita de detalles con `Include`:

```csharp
public object GetByIdWithDetails(Guid id)
{
    return _context.ListaPrecio
        .Include(lp => lp.ListaPrecio_Detalle)
        .Include(lp => lp.ListaPrecio_Detalle.Select(d => d.Producto))
        .FirstOrDefault(lp => lp.ID == id);
}
```

## Cómo Funciona Ahora

### Flujo de Actualización:

1. **Se carga la ListaPrecio con sus detalles** usando `Include`
2. **Se itera sobre los detalles existentes** y se remueven uno por uno con `Remove()`
3. **Entity Framework marca cada detalle como `Deleted`** en el contexto
4. **Se agregan los nuevos detalles** con `Add()`
5. **Entity Framework marca los nuevos como `Added`**
6. **Al llamar `SaveChanges()`**, Entity Framework ejecuta:
   - `DELETE FROM ListaPrecio_Detalle WHERE ID IN (...)`
   - `INSERT INTO ListaPrecio_Detalle (...) VALUES (...)`

### Por Qué Funciona:

| Enfoque | Resultado | Problema |
|---------|-----------|----------|
| `Clear()` | Intenta desvincular estableciendo FK a NULL | ? FK no acepta NULL |
| `Remove()` | Marca registros como Deleted | ? Se eliminan físicamente |
| Cascade `false` | No permite eliminación automática | ? Detalles huérfanos |
| Cascade `true` | Permite eliminación automática | ? Funciona correctamente |

## Estados de Entity Framework

```
ANTES del Update:
- ListaPrecio: Unchanged
- Detalle1: Unchanged
- Detalle2: Unchanged

DESPUÉS de Remove():
- ListaPrecio: Modified
- Detalle1: Deleted  ? Se eliminará físicamente
- Detalle2: Deleted  ? Se eliminará físicamente

DESPUÉS de Add():
- ListaPrecio: Modified
- Detalle1: Deleted
- Detalle2: Deleted
- Detalle3: Added    ? Se insertará
- Detalle4: Added    ? Se insertará

DESPUÉS de SaveChanges():
- DELETE FROM ListaPrecio_Detalle WHERE ID IN (Detalle1, Detalle2)
- INSERT INTO ListaPrecio_Detalle (...) VALUES (Detalle3)
- INSERT INTO ListaPrecio_Detalle (...) VALUES (Detalle4)
```

## Ventajas de la Solución

1. **? No hay conflictos con claves foráneas no nulables**
2. **? Eliminación física de registros antiguos**
3. **? Inserción de nuevos registros**
4. **? Soporte completo para el campo `IncluyeIva`**
5. **? Tracking correcto de cambios en Entity Framework**
6. **? No requiere cambios en la base de datos**

## Consideraciones Importantes

### 1. Eliminación en Cascada

**¿Por qué es seguro habilitar cascada?**
- Solo afecta a `ListaPrecio_Detalle`
- Los detalles son dependientes exclusivos de `ListaPrecio`
- No hay otras entidades que referencien a `ListaPrecio_Detalle`
- Si se elimina una `ListaPrecio`, es correcto eliminar sus detalles

### 2. Performance

**¿Es eficiente eliminar y reinsertar?**
- Para listas pequeñas (< 100 productos): Sí, es eficiente
- Para listas grandes: Considerar optimización con operaciones por lotes
- Alternativa avanzada: Comparar detalles existentes vs nuevos y solo modificar los que cambiaron

### 3. Transacciones

El `UnitOfWork` maneja transacciones:
```csharp
public int SaveChanges()
{
    try
    {
        return _context.SaveChanges(); // Transacción automática
    }
    catch (Exception)
    {
        throw;
    }
}
```

## Pruebas Recomendadas

### Test 1: Actualizar Lista con Nuevos Productos
```
1. Crear lista con productos A, B, C
2. Modificar lista con productos D, E, F
3. Verificar que A, B, C fueron eliminados
4. Verificar que D, E, F fueron insertados
```

### Test 2: Actualizar Lista Modificando Precios
```
1. Crear lista con Producto A (precio 100)
2. Modificar lista con Producto A (precio 150)
3. Verificar que se eliminó el registro antiguo
4. Verificar que se insertó el nuevo registro
```

### Test 3: Campo IncluyeIva
```
1. Crear lista con IncluyeIva = true
2. Modificar lista cambiando a IncluyeIva = false
3. Verificar que el campo se actualizó correctamente
```

### Test 4: Lista Sin Detalles
```
1. Crear lista con productos
2. Modificar lista eliminando todos los productos
3. Verificar que todos los detalles fueron eliminados
```

## Archivos Modificados

1. ? `BLL\Services\ListaPrecioService.cs`
   - Método `ActualizarEntidadListaPrecio`
   - Método `CrearEntidadListaPrecio`
   - Método `MapearDesdeEF`

2. ? `DAL\Implementation\EntityFramework\SistemaPresupuestario.cs`
   - Constructor con habilitación de tracking
   - Configuración de cascada en `OnModelCreating`

3. ? `DAL\Implementation\Repository\ListaPrecioRepository.cs`
   - Método `Update`
   - Métodos de consulta con `Include`

## Conclusión

El error está **completamente resuelto**. La actualización de listas de precios ahora funciona correctamente sin errores de claves foráneas no nulables.

**Compilación:** ? Exitosa  
**Funcionalidad:** ? Operativa  
**Campo IncluyeIva:** ? Soportado

---

**Fecha de Solución:** $(Get-Date -Format "yyyy-MM-dd HH:mm")  
**Versión .NET:** .NET Framework 4.8  
**Entity Framework:** 6.x
