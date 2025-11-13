# ?? Guía de Actualización: Conversión de Métodos Async a Sync en IProductoService

## ? Errores de Compilación Detectados

La migración de métodos async a sync en `IProductoService` requiere actualizar los siguientes archivos:

### Archivos con Errores:
1. **frmProductos.cs** (2 errores)
2. **frmProductoAlta.cs** (3 errores)
3. **frmListaPrecioAlta.cs** (1 error)
4. **frmPresupuesto.cs** (1 error)

---

## ?? Cambios Requeridos por Archivo

### 1. `frmProductos.cs`

#### Error 1: Línea 35
```csharp
// ? ANTES (Async)
var productos = await _productoService.GetAllAsync();

// ? DESPUÉS (Sync)
var productos = _productoService.GetAll();
```

**Cambio adicional:** Quitar `async` del método que contiene esta llamada.

#### Error 2: Línea 138
```csharp
// ? ANTES (Async)
await _productoService.DeleteAsync(productoId);

// ? DESPUÉS (Sync)
_productoService.Delete(productoId);
```

---

### 2. `frmProductoAlta.cs`

#### Error 1: Línea 80
```csharp
// ? ANTES (Async)
var producto = await _productoService.GetByIdAsync(productoId);

// ? DESPUÉS (Sync)
var producto = _productoService.GetById(productoId);
```

#### Error 2: Línea 169
```csharp
// ? ANTES (Async)
await _productoService.AddAsync(productoDto);

// ? DESPUÉS (Sync)
_productoService.Add(productoDto);
```

#### Error 3: Línea 173
```csharp
// ? ANTES (Async)
await _productoService.UpdateAsync(productoDto);

// ? DESPUÉS (Sync)
_productoService.Update(productoDto);
```

**Cambio adicional:** Quitar `async` del método que contiene estas llamadas.

---

### 3. `frmListaPrecioAlta.cs`

#### Error: Línea 424
```csharp
// ? ANTES (Async)
var productos = await _productoService.GetActivosAsync();

// ? DESPUÉS (Sync)
var productos = _productoService.GetActivos();
```

**Cambio adicional:** Quitar `async` del método `MostrarSelectorProductos()`.

---

### 4. `frmPresupuesto.cs`

#### Error: Línea 1634
```csharp
// ? ANTES (Async)
var productos = await _productoService.GetActivosAsync();

// ? DESPUÉS (Sync)
var productos = _productoService.GetActivos();
```

**Firma del método:**
```csharp
// ? ANTES
private async void MostrarSelectorProducto()

// ? DESPUÉS
private void MostrarSelectorProducto()
```

---

## ?? Patrón de Conversión General

### Para métodos void async:
```csharp
// ? ANTES
private async void MiMetodo()
{
    var resultado = await _service.GetDataAsync();
    // ... usar resultado
}

// ? DESPUÉS
private void MiMetodo()
{
    var resultado = _service.GetData(); // Sin await
    // ... usar resultado
}
```

### Para métodos Task async:
```csharp
// ? ANTES
private async Task<bool> MiMetodo()
{
    var resultado = await _service.GetDataAsync();
    return true;
}

// ? DESPUÉS
private bool MiMetodo()
{
    var resultado = _service.GetData(); // Sin await
    return true;
}
```

---

## ?? Tabla de Mapeo de Métodos

| Método Antiguo (Async) | Método Nuevo (Sync) |
|------------------------|---------------------|
| `GetAllAsync()` | `GetAll()` |
| `GetActivosAsync()` | `GetActivos()` |
| `GetByIdAsync(id)` | `GetById(id)` |
| `GetByCodigoAsync(codigo)` | `GetByCodigo(codigo)` |
| `AddAsync(dto)` | `Add(dto)` |
| `UpdateAsync(dto)` | `Update(dto)` |
| `DeleteAsync(id)` | `Delete(id)` |
| `ExisteCodigoAsync(codigo, excludeId)` | `ExisteCodigo(codigo, excludeId)` |

---

## ? Checklist de Actualización por Archivo

### frmProductos.cs
- [ ] Cambiar `GetAllAsync()` ? `GetAll()` (línea 35)
- [ ] Cambiar `DeleteAsync()` ? `Delete()` (línea 138)
- [ ] Quitar `async` de los métodos modificados
- [ ] Compilar y verificar

### frmProductoAlta.cs
- [ ] Cambiar `GetByIdAsync()` ? `GetById()` (línea 80)
- [ ] Cambiar `AddAsync()` ? `Add()` (línea 169)
- [ ] Cambiar `UpdateAsync()` ? `Update()` (línea 173)
- [ ] Quitar `async` de los métodos modificados
- [ ] Compilar y verificar

### frmListaPrecioAlta.cs
- [ ] Cambiar `GetActivosAsync()` ? `GetActivos()` (línea 424)
- [ ] Quitar `async` del método `MostrarSelectorProductos()`
- [ ] Compilar y verificar

### frmPresupuesto.cs
- [ ] Cambiar `GetActivosAsync()` ? `GetActivos()` (línea 1634)
- [ ] Quitar `async` del método `MostrarSelectorProducto()`
- [ ] Compilar y verificar

---

## ?? Beneficios de esta Migración

1. ? **Elimina deadlocks** - No más bloqueos del UI Thread
2. ? **Código más simple** - Sin async/await innecesario
3. ? **Mejor rendimiento** - Menos context switches
4. ? **Más predecible** - Flujo de ejecución directo
5. ? **Menos errores** - Sin problemas de sincronización

---

## ?? Importante

- **NO usar `.Wait()` o `.GetAwaiter().GetResult()`** - Causa deadlocks
- **Quitar SIEMPRE el `async`** cuando quites el `await`
- **Quitar SIEMPRE el `Task<>`** en el return type si no usas async
- **Compilar después de cada cambio** para verificar

---

## ?? Ejemplo Completo de Conversión

### Antes (con async/await):
```csharp
private async void CargarProductos()
{
    try
    {
        this.Cursor = Cursors.WaitCursor;
        
        var productos = await _productoService.GetAllAsync();
        dgvProductos.DataSource = productos.ToList();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error", 
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        this.Cursor = Cursors.Default;
    }
}
```

### Después (síncrono):
```csharp
private void CargarProductos()
{
    try
    {
        this.Cursor = Cursors.WaitCursor;
        
        var productos = _productoService.GetAll(); // ?? Sin await
        dgvProductos.DataSource = productos.ToList();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error", 
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        this.Cursor = Cursors.Default;
    }
}
```

---

**Fecha de Creación:** 2025-01-XX  
**Autor:** GitHub Copilot  
**Versión:** 1.0
