# Instrucciones para completar la funcionalidad IncluyeIva

## 1. Ejecutar el script SQL
Ejecutar el archivo `Scripts/AgregarCampoIncluyeIva.sql` en la base de datos para agregar el campo `IncluyeIva` a la tabla `ListaPrecio`.

## 2. Modificar frmListaPrecioAlta.cs

Buscar el método `btnAceptar_Click` (línea aproximada 432) y reemplazar esta sección:

```csharp
var dto = new ListaPrecioDTO
{
    Codigo = txtCodigo.Text.Trim(),
    Nombre = txtNombre.Text.Trim(),
    Activo = chkActivo.Checked,
    Detalles = detallesValidos
};
```

Por:

```csharp
var dto = new ListaPrecioDTO
{
    Codigo = txtCodigo.Text.Trim(),
    Nombre = txtNombre.Text.Trim(),
    Activo = chkActivo.Checked,
    IncluyeIva = chkIncluyeIva.Checked,
    Detalles = detallesValidos
};
```

## 3. Actualizar el formulario de presupuestos (frmPresupuesto.cs)

### 3.1. Agregar campo privado para almacenar si la lista incluye IVA

Buscar la línea (aproximada 30):
```csharp
private Guid? _idListaPrecioSeleccionada = null;
```

Agregar después:
```csharp
private bool _listaPrecioIncluyeIva = false;
```

### 3.2. Actualizar el método `CargarListaPrecioDelPresupuesto`

Buscar el método `CargarListaPrecioDelPresupuesto` (línea aproximada 402) y reemplazar:

```csharp
private async void CargarListaPrecioDelPresupuesto(Guid? idListaPrecio)
{
    try
    {
        if (idListaPrecio.HasValue)
        {
            var lista = await _listaPrecioService.GetByIdAsync(idListaPrecio.Value);
            if (lista != null)
            {
                txtCodigoListaPrecio.Text = lista.Codigo;
                txtListaPrecio.Text = lista.Nombre;
            }
            else
            {
                txtCodigoListaPrecio.Clear();
                txtListaPrecio.Clear();
            }
        }
        else
        {
            txtCodigoListaPrecio.Clear();
            txtListaPrecio.Clear();
        }
    }
    catch
    {
        // Si hay error, limpiar los campos
        txtCodigoListaPrecio.Clear();
        txtListaPrecio.Clear();
    }
}
```

Por:

```csharp
private async void CargarListaPrecioDelPresupuesto(Guid? idListaPrecio)
{
    try
    {
        if (idListaPrecio.HasValue)
        {
            var lista = await _listaPrecioService.GetByIdAsync(idListaPrecio.Value);
            if (lista != null)
            {
                txtCodigoListaPrecio.Text = lista.Codigo;
                txtListaPrecio.Text = lista.Nombre;
                _listaPrecioIncluyeIva = lista.IncluyeIva;
            }
            else
            {
                txtCodigoListaPrecio.Clear();
                txtListaPrecio.Clear();
                _listaPrecioIncluyeIva = false;
            }
        }
        else
        {
            txtCodigoListaPrecio.Clear();
            txtListaPrecio.Clear();
            _listaPrecioIncluyeIva = false;
        }
    }
    catch
    {
        // Si hay error, limpiar los campos
        txtCodigoListaPrecio.Clear();
        txtListaPrecio.Clear();
        _listaPrecioIncluyeIva = false;
    }
}
```

### 3.3. Actualizar el método `LimpiarFormulario`

Buscar la línea (aproximada 455):
```csharp
_idListaPrecioSeleccionada = null; // NUEVO
```

Agregar después:
```csharp
_listaPrecioIncluyeIva = false;
```

### 3.4. Actualizar método `txtCodigoListaPrecio_Leave`

Buscar el método `txtCodigoListaPrecio_Leave` (línea aproximada 1610) y reemplazar:

```csharp
if (lista != null)
{
    // Lista de precios encontrada - guardar ID y mostrar datos
    _idListaPrecioSeleccionada = lista.Id;
    txtCodigoListaPrecio.Text = lista.Codigo;
    txtListaPrecio.Text = lista.Nombre;
}
```

Por:

```csharp
if (lista != null)
{
    // Lista de precios encontrada - guardar ID y mostrar datos
    _idListaPrecioSeleccionada = lista.Id;
    txtCodigoListaPrecio.Text = lista.Codigo;
    txtListaPrecio.Text = lista.Nombre;
    _listaPrecioIncluyeIva = lista.IncluyeIva;
}
```

### 3.5. Actualizar el método `MostrarSelectorListaPrecio`

Buscar en el método `MostrarSelectorListaPrecio` (línea aproximada 1678):

```csharp
if (listaSeleccionada != null)
{
    // Guardar ID
    _idListaPrecioSeleccionada = listaSeleccionada.Id;

    // Aplicar la lista de precios seleccionada
    txtCodigoListaPrecio.Text = listaSeleccionada.Codigo;
    txtListaPrecio.Text = listaSeleccionada.Nombre;
}
```

Reemplazar por:

```csharp
if (listaSeleccionada != null)
{
    // Guardar ID
    _idListaPrecioSeleccionada = listaSeleccionada.Id;

    // Aplicar la lista de precios seleccionada
    txtCodigoListaPrecio.Text = listaSeleccionada.Codigo;
    txtListaPrecio.Text = listaSeleccionada.Nombre;
    _listaPrecioIncluyeIva = listaSeleccionada.IncluyeIva;
}
```

Y también en la parte del else (cancelación):

```csharp
else
{
    // Usuario canceló - limpiar campos
    txtCodigoListaPrecio.Clear();
    txtListaPrecio.Clear();
    _idListaPrecioSeleccionada = null;
}
```

Por:

```csharp
else
{
    // Usuario canceló - limpiar campos
    txtCodigoListaPrecio.Clear();
    txtListaPrecio.Clear();
    _idListaPrecioSeleccionada = null;
    _listaPrecioIncluyeIva = false;
}
```

### 3.6. Actualizar el método `ValidarYMoverFocoListaPrecio`

Buscar en el método `ValidarYMoverFocoListaPrecio` (línea aproximada 1718):

```csharp
if (lista != null)
{
    // Lista de precios encontrada - guardar ID y aplicar datos
    _idListaPrecioSeleccionada = lista.Id;
    txtCodigoListaPrecio.Text = lista.Codigo;
    txtListaPrecio.Text = lista.Nombre;

    // Mover al siguiente control
    this.SelectNextControl(txtCodigoListaPrecio, true, true, true, true);
}
```

Reemplazar por:

```csharp
if (lista != null)
{
    // Lista de precios encontrada - guardar ID y aplicar datos
    _idListaPrecioSeleccionada = lista.Id;
    txtCodigoListaPrecio.Text = lista.Codigo;
    txtListaPrecio.Text = lista.Nombre;
    _listaPrecioIncluyeIva = lista.IncluyeIva;

    // Mover al siguiente control
    this.SelectNextControl(txtCodigoListaPrecio, true, true, true, true);
}
```

## 4. Modificar PresupuestoDetalleDTO.cs para agregar lógica de cálculo de IVA

Este cambio es CRÍTICO para aplicar la nueva lógica de cálculo.

Buscar el archivo `BLL/DTOs/PresupuestoDetalleDTO.cs` y localizar la propiedad `Iva`:

```csharp
public decimal Iva => Total * (PorcentajeIVA / 100);
```

**NOTA IMPORTANTE**: Esta lógica debe cambiar, pero necesita recibir el valor de `IncluyeIva` desde el presupuesto. 

La forma correcta es modificar el método `CalcularTotales()` en `frmPresupuesto.cs` para que considere si la lista de precios incluye IVA.

Buscar el método `CalcularTotales` (línea aproximada 633) y reemplazar por:

```csharp
private void CalcularTotales()
{
    try
    {
        decimal subtotal = 0;
        decimal iva = 0;
        decimal total = 0;

        foreach (var detalle in _detalles)
        {
            if (_listaPrecioIncluyeIva)
            {
                // Si la lista incluye IVA, el Total ya tiene el IVA incluido
                // Por lo tanto: IVA = Total - (Total / 1.21)
                decimal totalConIva = detalle.Total;
                decimal totalSinIva = totalConIva / 1.21m;
                decimal ivaDetalle = totalConIva - totalSinIva;
                
                subtotal += totalSinIva;
                iva += ivaDetalle;
                total += totalConIva;
            }
            else
            {
                // Lógica actual: el precio NO incluye IVA
                subtotal += detalle.Total;
                iva += detalle.Iva;
                total += detalle.TotalConIva;
            }
        }

        txtSUB.Text = subtotal.ToString("N2");
        txtIva.Text = iva.ToString("N2");
        txtTotal.Text = total.ToString("N2");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error al calcular totales: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

## 5. Compilar y probar

1. Compilar la solución
2. Verificar que no haya errores
3. Probar la funcionalidad:
   - Crear una lista de precios con "Incluye IVA" marcado
   - Crear un presupuesto usando esa lista
   - Verificar que los totales se calculen correctamente

## Notas Importantes

- La fórmula para calcular el IVA cuando está incluido es: `IVA = Total - (Total / 1.21)`
- Esta fórmula asume que el IVA es del 21% (1.21 = 1 + 0.21)
- Si hay productos con diferentes porcentajes de IVA, la fórmula debería ajustarse por detalle usando `(Total / (1 + PorcentajeIVA/100))`
