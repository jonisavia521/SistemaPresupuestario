# Script de Actualización Manual para Archivos Restantes

## ?? INSTRUCCIONES

Debido al tamaño de los archivos, necesitas hacer estos cambios manualmente.  
Sigue exactamente estas instrucciones para cada archivo.

---

## ?? **Archivo 1: frmProductoAlta.cs**

### Cambios a realizar:

#### 1. Buscar la línea ~80:
```csharp
var producto = await _productoService.GetByIdAsync(_productoId.Value);
```
**Cambiar a:**
```csharp
var producto = _productoService.GetById(_productoId.Value);
```

#### 2. Cambiar la firma del método que contiene esa línea:
```csharp
// ANTES:
private async void frmProductoAlta_Load(object sender, EventArgs e)

// DESPUÉS:
private void frmProductoAlta_Load(object sender, EventArgs e)
```

#### 3. Buscar la línea ~169:
```csharp
await _productoService.AddAsync(productoDto);
```
**Cambiar a:**
```csharp
_productoService.Add(productoDto);
```

#### 4. Buscar la línea ~173:
```csharp
await _productoService.UpdateAsync(productoDto);
```
**Cambiar a:**
```csharp
_productoService.Update(productoDto);
```

#### 5. Cambiar la firma del método btnAceptar_Click:
```csharp
// ANTES:
private async void btnAceptar_Click(object sender, EventArgs e)

// DESPUÉS:
private void btnAceptar_Click(object sender, EventArgs e)
```

#### 6. Remover import:
```csharp
// QUITAR esta línea al inicio del archivo:
using System.Threading.Tasks;
```

---

## ?? **Archivo 2: frmListaPrecioAlta.cs**

### Cambios a realizar:

#### 1. Buscar la línea ~424 (método MostrarSelectorProductos):
```csharp
var productos = await _productoService.GetActivosAsync();
```
**Cambiar a:**
```csharp
var productos = _productoService.GetActivos();
```

#### 2. Cambiar la firma del método:
```csharp
// ANTES:
private async void MostrarSelectorProductos()

// DESPUÉS:
private void MostrarSelectorProductos()
```

---

## ?? **Archivo 3: frmPresupuesto.cs**

### Cambios a realizar:

#### 1. Buscar el método `MostrarSelectorProducto` (línea ~1634):
```csharp
// ANTES:
private async void MostrarSelectorProducto()
{
    try
    {
        // Obtener todos los productos activos
        var productos = await _productoService.GetActivosAsync();
        var listaProductos = productos.ToList();
        
        // ...resto del código...
    }
}

// DESPUÉS:
private void MostrarSelectorProducto()
{
    try
    {
        // Obtener todos los productos activos
        var productos = _productoService.GetActivos(); // ?? SIN await
        var listaProductos = productos.ToList();
        
        // ...resto del código...
    }
}
```

#### 2. Buscar en el mismo método las líneas de obtención de precio:
```csharp
// MANTENER tal cual (esta parte con .Wait() está OK temporalmente)
var precioTask = _listaPrecioService.ObtenerPrecioProductoAsync(
    _idListaPrecioSeleccionada.Value, 
    productoSeleccionado.Id);
precioTask.Wait();

if (precioTask.Result.HasValue)
{
    precioDefault = precioTask.Result.Value;
}
```
**NOTA:** Esta parte se corregirá cuando migremos ListaPrecioService.

---

## ? Verificación Final

Después de hacer todos los cambios:

### 1. Compilar el proyecto:
```
Build > Build Solution (Ctrl+Shift+B)
```

### 2. Verificar que no haya errores relacionados con:
- `GetAllAsync`
- `GetActivosAsync`
- `GetByIdAsync`
- `AddAsync`
- `UpdateAsync`
- `DeleteAsync`

### 3. Ejecutar la aplicación y probar:
- ? Abrir lista de productos
- ? Crear un producto nuevo
- ? Editar un producto existente
- ? Buscar un producto en el presupuesto
- ? Inhabilitar un producto

---

## ?? Solución de Problemas

### Error: "CS1061: does not contain a definition for 'GetAllAsync'"
**Solución:** Cambiaste `GetAllAsync()` a `GetAll()` pero olvidaste quitar el `await`.
```csharp
// MAL:
var productos = await _productoService.GetAll(); // ? await sin async

// BIEN:
var productos = _productoService.GetAll(); // ?
```

### Error: "CS1998: This async method lacks 'await' operators"
**Solución:** Quitaste el `await` pero olvidaste quitar el `async` del método.
```csharp
// MAL:
private async void MiMetodo() // ? async sin await
{
    var productos = _productoService.GetAll();
}

// BIEN:
private void MiMetodo() // ?
{
    var productos = _productoService.GetAll();
}
```

### La aplicación se "cuelga" al buscar productos
**Posible causa:** No quitaste el `await` correctamente.
**Solución:** Verifica que TODOS los métodos async se hayan convertido a sync.

---

## ?? ¿Necesitas Ayuda?

Si encuentras algún problema, verifica:
1. Que quitaste **TODOS** los `await`
2. Que quitaste **TODOS** los `async`
3. Que cambiaste **TODOS** los nombres de métodos (Async ? sin Async)
4. Que compilaste después de cada cambio

---

**Última Actualización:** 2025-01-XX  
**Autor:** GitHub Copilot
