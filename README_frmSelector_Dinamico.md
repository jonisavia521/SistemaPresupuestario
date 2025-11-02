# frmSelector - Formulario Dinámico y Genérico

## Descripción

`frmSelector` es un componente reutilizable que permite seleccionar elementos de cualquier lista de objetos. Es completamente dinámico y configurable mediante el patrón de configuración `SelectorConfig<T>`.

## Características

? **Genérico**: Funciona con cualquier tipo de objeto  
? **Configurable**: Columnas, filtros y comportamiento personalizables  
? **Búsqueda**: Filtro en tiempo real  
? **Selección Simple o Múltiple**: Configurable según necesidad  
? **Tipado Seguro**: Usa genéricos de C# para type-safety  
? **Reutilizable**: Se puede usar desde cualquier formulario  

## Ubicación

```
SistemaPresupuestario\Maestros\Shared\
??? frmSelector.cs              (Lógica del formulario)
??? frmSelector.Designer.cs     (Diseño del formulario)
??? SelectorConfig.cs           (Clases de configuración)
```

## Uso Básico

### 1. Selección Simple de Vendedores

```csharp
private void AbrirSelectorVendedor()
{
    // Configurar el selector
    var config = new SelectorConfig<VendedorDTO>
    {
        Titulo = "Seleccionar Vendedor",
        Datos = await _vendedorService.GetActivosAsync(),
        PlaceholderBusqueda = "Buscar por código, nombre o CUIT...",
        
        // Definir columnas
        Columnas = new List<ColumnaConfig>
        {
            new ColumnaConfig { NombrePropiedad = "Id", Visible = false },
            new ColumnaConfig { NombrePropiedad = "CodigoVendedor", TituloColumna = "Código", Ancho = 80 },
            new ColumnaConfig { NombrePropiedad = "Nombre", TituloColumna = "Nombre", Ancho = 250 },
            new ColumnaConfig { NombrePropiedad = "CUIT", TituloColumna = "CUIT", Ancho = 150 }
        },
        
        // Función de filtro personalizada
        FuncionFiltro = (busqueda, vendedor) =>
        {
            var b = busqueda.ToUpper();
            return (vendedor.CodigoVendedor?.Contains(b) ?? false) ||
                   (vendedor.Nombre?.ToUpper().Contains(b) ?? false) ||
                   (vendedor.CUIT?.Contains(b) ?? false);
        }
    };

    // Mostrar el selector
    using (var selector = frmSelector.Mostrar(config))
    {
        if (selector.ShowDialog() == DialogResult.OK && selector.ElementoSeleccionado != null)
        {
            var vendedor = (VendedorDTO)selector.ElementoSeleccionado;
            // Usar el vendedor seleccionado
            MessageBox.Show($"Seleccionó: {vendedor.Nombre}");
        }
    }
}
```

### 2. Selección Simple de Productos

```csharp
private void AbrirSelectorProducto()
{
    var config = new SelectorConfig<ProductoDTO>
    {
        Titulo = "Seleccionar Producto",
        Datos = await _productoService.GetActivosAsync(),
        PlaceholderBusqueda = "Buscar producto...",
        
        Columnas = new List<ColumnaConfig>
        {
            new ColumnaConfig { NombrePropiedad = "Id", Visible = false },
            new ColumnaConfig { NombrePropiedad = "Codigo", TituloColumna = "Código", Ancho = 100 },
            new ColumnaConfig { NombrePropiedad = "Descripcion", TituloColumna = "Descripción", Ancho = 300 },
            new ColumnaConfig { NombrePropiedad = "EstadoTexto", TituloColumna = "Estado", Ancho = 100 }
        }
    };

    using (var selector = frmSelector.Mostrar(config))
    {
        if (selector.ShowDialog() == DialogResult.OK)
        {
            var producto = (ProductoDTO)selector.ElementoSeleccionado;
            // Usar el producto seleccionado
        }
    }
}
```

### 3. Selección Múltiple de Clientes

```csharp
private void AbrirSelectorClientesMultiple()
{
    var config = new SelectorConfig<ClienteDTO>
    {
        Titulo = "Seleccionar Clientes",
        Datos = await _clienteService.GetActivosAsync(),
        PermitirSeleccionMultiple = true, // ? Activar selección múltiple
        
        Columnas = new List<ColumnaConfig>
        {
            new ColumnaConfig { NombrePropiedad = "Id", Visible = false },
            new ColumnaConfig { NombrePropiedad = "CodigoCliente", TituloColumna = "Código", Ancho = 100 },
            new ColumnaConfig { NombrePropiedad = "RazonSocial", TituloColumna = "Razón Social", Ancho = 250 },
            new ColumnaConfig { NombrePropiedad = "DocumentoCompleto", TituloColumna = "Documento", Ancho = 150 }
        }
    };

    using (var selector = frmSelector.Mostrar(config))
    {
        if (selector.ShowDialog() == DialogResult.OK)
        {
            var clientes = selector.ElementosSeleccionados.Cast<ClienteDTO>().ToList();
            MessageBox.Show($"Seleccionó {clientes.Count} clientes");
        }
    }
}
```

### 4. Selector con Auto-Generación de Columnas

```csharp
private void AbrirSelectorSimple()
{
    var config = new SelectorConfig<MiClaseDTO>
    {
        Titulo = "Seleccionar Item",
        Datos = miLista,
        // No especificar Columnas ? se auto-generan todas
    };

    using (var selector = frmSelector.Mostrar(config))
    {
        if (selector.ShowDialog() == DialogResult.OK)
        {
            var item = (MiClaseDTO)selector.ElementoSeleccionado;
        }
    }
}
```

### 5. Filtro Genérico (sin FuncionFiltro)

Si no especificas `FuncionFiltro`, el selector usa un filtro genérico que busca en todas las propiedades de tipo `string`:

```csharp
var config = new SelectorConfig<ProductoDTO>
{
    Titulo = "Seleccionar Producto",
    Datos = productos,
    // Sin FuncionFiltro ? busca automáticamente en todas las propiedades string
};
```

## Configuración Detallada

### SelectorConfig<T>

| Propiedad | Tipo | Descripción | Obligatorio |
|-----------|------|-------------|-------------|
| `Titulo` | `string` | Título de la ventana | No (default: "Selector") |
| `Datos` | `IEnumerable<T>` | Lista de objetos a mostrar | Sí |
| `Columnas` | `List<ColumnaConfig>` | Definición de columnas | No (auto-genera si está vacío) |
| `FuncionFiltro` | `Func<string, T, bool>` | Función personalizada de filtrado | No (usa filtro genérico) |
| `PlaceholderBusqueda` | `string` | Texto del label de búsqueda | No (default: "Buscar...") |
| `PermitirSeleccionMultiple` | `bool` | Permite seleccionar múltiples items | No (default: false) |

### ColumnaConfig

| Propiedad | Tipo | Descripción | Obligatorio |
|-----------|------|-------------|-------------|
| `NombrePropiedad` | `string` | Nombre de la propiedad del objeto | Sí |
| `TituloColumna` | `string` | Texto del encabezado de columna | No (usa NombrePropiedad) |
| `Ancho` | `int` | Ancho de la columna en píxeles | No (default: 100) |
| `Visible` | `bool` | Si la columna es visible | No (default: true) |

## Funcionalidad de Filtrado

### Filtro Personalizado

```csharp
FuncionFiltro = (busqueda, objeto) =>
{
    var b = busqueda.ToUpper();
    
    // Lógica de filtrado personalizada
    return objeto.Campo1.ToUpper().Contains(b) ||
           objeto.Campo2.ToUpper().Contains(b) ||
           objeto.Campo3.Contains(b);
}
```

### Filtro Genérico (Automático)

Si no especificas `FuncionFiltro`, el selector:
1. Itera todas las propiedades del objeto
2. Busca solo en propiedades de tipo `string`
3. Compara (case-insensitive) si contiene el texto de búsqueda
4. Retorna el objeto si al menos una propiedad coincide

## Interacción del Usuario

### Selección Simple
- ? Click en una fila + botón "Seleccionar"
- ? Doble-click en una fila
- ? Enter sobre la fila seleccionada

### Selección Múltiple
- ? Ctrl + Click para agregar/quitar filas
- ? Shift + Click para rango
- ? Botón "Seleccionar" para confirmar

### Búsqueda
- ? Filtro en tiempo real al escribir
- ? Actualización automática de contador de resultados
- ? Auto-selección del primer resultado

## Propiedades de Resultado

### Selección Simple
```csharp
selector.ElementoSeleccionado // ? object (hacer cast al tipo esperado)
```

### Selección Múltiple
```csharp
selector.ElementosSeleccionados // ? List<object> (hacer cast al tipo esperado)
```

## Ventajas del Diseño

### 1. **Reutilizable**
Un solo formulario para todos los casos de selección en la aplicación.

### 2. **Type-Safe**
Usa genéricos de C# para evitar errores de tipo en tiempo de compilación.

### 3. **Configurable**
Cada uso puede personalizar columnas, filtros y comportamiento.

### 4. **Mantenible**
Cambios en el selector se propagan a toda la aplicación.

### 5. **Consistente**
UX uniforme en toda la aplicación.

## Ejemplos Avanzados

### Selector con Cálculos en Columnas

```csharp
var config = new SelectorConfig<VentaDTO>
{
    Titulo = "Seleccionar Venta",
    Datos = ventas,
    
    Columnas = new List<ColumnaConfig>
    {
        new ColumnaConfig { NombrePropiedad = "NumeroVenta", TituloColumna = "Nº", Ancho = 80 },
        new ColumnaConfig { NombrePropiedad = "Cliente", TituloColumna = "Cliente", Ancho = 200 },
        new ColumnaConfig { NombrePropiedad = "TotalFormateado", TituloColumna = "Total", Ancho = 120 }
        // TotalFormateado es una propiedad calculada en el DTO
    }
};
```

### Selector con Filtro Complejo

```csharp
FuncionFiltro = (busqueda, cliente) =>
{
    var b = busqueda.ToUpper();
    
    // Buscar por código, razón social, CUIT o email
    return (cliente.CodigoCliente?.ToUpper().Contains(b) ?? false) ||
           (cliente.RazonSocial?.ToUpper().Contains(b) ?? false) ||
           (cliente.NumeroDocumento?.Contains(b) ?? false) ||
           (cliente.Email?.ToUpper().Contains(b) ?? false);
}
```

## Integración con el Sistema

El `frmSelector` ya está integrado en:

? **frmClienteAlta**: Selección de vendedor (KeyDown en txtCodigoVendedor)  

### Próximas Integraciones Sugeridas

- [ ] **frmPresupuestoAlta**: Selección de cliente y productos
- [ ] **frmVentaAlta**: Selección de cliente
- [ ] **frmFacturaAlta**: Selección de presupuesto a facturar
- [ ] **Cualquier formulario**: Donde necesites seleccionar de una lista

## Notas Técnicas

### Por qué no usa IServiceProvider

`frmSelector` es un formulario completamente agnóstico de servicios. No necesita inyección de dependencias porque:

1. Recibe los datos ya cargados (`IEnumerable<T>`)
2. Es responsabilidad del formulario llamador obtener los datos
3. Esto lo hace más reutilizable y testeable

### Patrón de Diseño

Implementa el patrón **Strategy** para el filtrado:
- El selector define la interfaz (`Func<string, T, bool>`)
- Cada uso provee la estrategia concreta
- Permite flexibilidad sin modificar el selector

---

**Autor**: Sistema  
**Fecha**: 2024  
**Versión**: 1.0  
**Estado**: ? Implementado y funcional
