# Guía de Uso del Sistema de Traducción Dinámica

## Descripción General

El sistema de traducción dinámica permite que la aplicación cambie automáticamente entre español e inglés según la configuración almacenada en la base de datos (tabla `Configuracion`, campo `Idioma`).

## Arquitectura

### Componentes Principales

1. **TranslationService** (`Services\Services\TranslationService.cs`)
   - Servicio central que carga y gestiona las traducciones
   - Se inicializa al inicio de la aplicación en `Program.cs`
   - Lee el archivo `Textos_Controles_UI.txt`

2. **I18n Helper** (`SistemaPresupuestario\Helpers\I18n.cs`)
   - Clase helper estática para facilitar el uso de traducciones
   - Método principal: `I18n.T("clave")` para traducir textos

3. **Archivo de Traducciones** (`SistemaPresupuestario\Textos_Controles_UI.txt`)
   - Formato: `clave=texto_español|texto_inglés`
   - Ejemplo: `Login=Login|Login`
   - Ejemplo: `Bienvenido=Bienvenido|Welcome`

## Formato del Archivo de Traducciones

```
clave=español|inglés
```

Ejemplo completo:
```
Login=Login|Login
Ingresar=Ingresar|Enter
Usuario o contraseña invalida=Usuario o contraseña inválida|Invalid username or password
ADVERTENCIA=ADVERTENCIA|WARNING
Bienvenido=Bienvenido|Welcome
```

## Cómo Usar en tus Formularios

### Paso 1: Agregar el using

```csharp
using SistemaPresupuestario.Helpers;
```

### Paso 2: Crear método de traducción en tu formulario

```csharp
private void AplicarTraducciones()
{
    try
    {
        // Título del formulario
        this.Text = I18n.T("Gestión de Clientes");
        
        // Botones
        btnNuevo.Text = I18n.T("Nuevo");
        btnEditar.Text = I18n.T("Editar");
        btnEliminar.Text = I18n.T("Eliminar");
        btnCerrar.Text = I18n.T("Cerrar");
        
        // Labels
        lblBuscar.Text = I18n.T("Buscar");
        
        // CheckBox
        chkSoloActivos.Text = I18n.T("Solo ver activos");
        
        // Columnas de DataGridView
        colCodigo.HeaderText = I18n.T("Código");
        colNombre.HeaderText = I18n.T("Nombre");
        colEstado.HeaderText = I18n.T("Estado");
        
        // GroupBox
        grpDatos.Text = I18n.T("Datos Básicos");
    }
    catch (Exception ex)
    {
        // En caso de error, continuar sin traducciones
        System.Diagnostics.Debug.WriteLine($"Error al aplicar traducciones: {ex.Message}");
    }
}
```

### Paso 3: Llamar al método en el evento Load

```csharp
private void frmClientes_Load(object sender, EventArgs e)
{
    // Aplicar traducciones
    AplicarTraducciones();
    
    // Resto del código de carga...
    CargarDatos();
}
```

### Paso 4: Usar en MessageBox y otros controles dinámicos

```csharp
// MessageBox
MessageBox.Show(
    I18n.T("Cliente guardado exitosamente"),
    I18n.T("Éxito"),
    MessageBoxButtons.OK,
    MessageBoxIcon.Information);

// Confirmación
var resultado = MessageBox.Show(
    I18n.T("¿Está seguro de eliminar este registro?"),
    I18n.T("Confirmar eliminación"),
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question);
```

## Ejemplos de Uso Completo

### Ejemplo 1: Formulario Simple

```csharp
using SistemaPresupuestario.Helpers;

public partial class frmVendedores : Form
{
    private void frmVendedores_Load(object sender, EventArgs e)
    {
        AplicarTraducciones();
        CargarDatos();
    }
    
    private void AplicarTraducciones()
    {
        this.Text = I18n.T("Gestión de Vendedores");
        btnNuevo.Text = I18n.T("Nuevo");
        btnEditar.Text = I18n.T("Editar");
        btnDesactivar.Text = I18n.T("Desactivar");
        btnCerrar.Text = I18n.T("Cerrar");
        
        // Columnas
        colCodigo.HeaderText = I18n.T("Código");
        colNombre.HeaderText = I18n.T("Nombre");
        colComision.HeaderText = I18n.T("Comisión %");
        colEstado.HeaderText = I18n.T("Estado");
    }
    
    private void btnEliminar_Click(object sender, EventArgs e)
    {
        var resultado = MessageBox.Show(
            I18n.T("¿Está seguro que desea desactivar el vendedor"),
            I18n.T("Confirmar Eliminación"),
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
            
        if (resultado == DialogResult.Yes)
        {
            // Lógica de eliminación
            MessageBox.Show(
                I18n.T("Vendedor desactivado exitosamente"),
                I18n.T("Éxito"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
```

### Ejemplo 2: Formulario con Pestañas

```csharp
using SistemaPresupuestario.Helpers;

public partial class frmConfiguracion : Form
{
    private void AplicarTraducciones()
    {
        this.Text = I18n.T("Configuración del Sistema");
        
        // Pestañas
        tabConfiguracion.Text = I18n.T("Configuración General");
        tabIdioma.Text = I18n.T("Idioma");
        tabBackup.Text = I18n.T("BackUp");
        
        // GroupBox
        grpEmpresa.Text = I18n.T("Datos de la Empresa");
        grpIdioma.Text = I18n.T("Selección de Idioma");
        
        // Labels
        lblRazonSocial.Text = I18n.T("Razón Social");
        lblCUIT.Text = I18n.T("CUIT");
        lblEmail.Text = I18n.T("Email");
        
        // Botones
        btnGuardar.Text = I18n.T("Guardar Configuración");
        btnCambiarIdioma.Text = I18n.T("Cambiar Idioma");
    }
}
```

## Agregar Nuevas Traducciones

### Paso 1: Editar el archivo `Textos_Controles_UI.txt`

Agregar una línea con el formato:
```
NuevaClaveEspañol=Texto en español|Text in english
```

### Paso 2: Usar en el código

```csharp
string textoTraducido = I18n.T("NuevaClaveEspañol");
```

## Métodos Adicionales del Helper I18n

```csharp
// Obtener el idioma actual
string idioma = I18n.GetCurrentLanguage(); // Retorna "es-AR" o "en-US"

// Verificar si es español
if (I18n.IsSpanish())
{
    // Código específico para español
}

// Verificar si es inglés
if (I18n.IsEnglish())
{
    // Código específico para inglés
}
```

## Cómo Cambiar el Idioma

El usuario puede cambiar el idioma desde:
1. **Menú Principal** → **Configuración** → **Pestaña Idioma**
2. Seleccionar el idioma deseado (Español/Inglés)
3. Presionar **Cambiar Idioma**
4. La aplicación se reiniciará automáticamente

El idioma se guarda en la tabla `Configuracion` y se carga automáticamente al iniciar la aplicación.

## Flujo de Inicialización

1. `Program.cs` → `Main()` inicia la aplicación
2. `InicializarIdioma()` lee el idioma de la BD
3. `TranslationService.Initialize()` carga el archivo de traducciones
4. Todos los formularios usan `I18n.T()` para traducir

## Solución de Problemas

### Problema: Las traducciones no aparecen

**Solución**: Verificar que:
1. El archivo `Textos_Controles_UI.txt` exista en la carpeta de salida (bin\Debug o bin\Release)
2. El formato del archivo sea correcto: `clave=español|inglés`
3. Se llame a `AplicarTraducciones()` en el evento `Load` del formulario

### Problema: Error al cargar traducciones

**Solución**: 
- El sistema está diseñado para ser tolerante a fallos
- Si hay error, devolverá la clave original sin traducir
- Revisar el archivo `Textos_Controles_UI.txt` en busca de errores de formato

### Problema: Necesito recargar las traducciones sin reiniciar

**Solución**: Por el momento, las traducciones se cargan al inicio. Para cambiar el idioma se requiere reiniciar la aplicación.

## Recomendaciones

1. **Consistencia**: Usar siempre las mismas claves para los mismos textos
2. **Nombres descriptivos**: Usar claves descriptivas: "Gestión de Clientes" en lugar de "GC"
3. **Agregar todas las traducciones**: Cada texto visible debe tener su traducción
4. **Probar en ambos idiomas**: Verificar que los textos traducidos caben en los controles
5. **Documentar**: Documentar las claves nuevas que agregues

## Notas Importantes

- El archivo de traducciones NO distingue mayúsculas/minúsculas en las claves
- Si una clave no existe, se devuelve la clave original
- El sistema es tolerante a errores y no interrumpirá el funcionamiento de la aplicación
- Los cambios en el archivo de traducciones requieren reiniciar la aplicación

## Ejemplo Completo: Formulario de Productos

```csharp
using SistemaPresupuestario.Helpers;
using System;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Productos
{
    public partial class frmProductos : Form
    {
        public frmProductos()
        {
            InitializeComponent();
        }

        private void frmProductos_Load(object sender, EventArgs e)
        {
            AplicarTraducciones();
            CargarProductos();
        }

        private void AplicarTraducciones()
        {
            try
            {
                // Formulario
                this.Text = I18n.T("Gestión de Productos");
                
                // Botones
                btnNuevo.Text = I18n.T("Nuevo");
                btnEditar.Text = I18n.T("Editar");
                btnInhabilitar.Text = I18n.T("Inhabilitar");
                btnCerrar.Text = I18n.T("Cerrar");
                
                // Labels
                lblBuscar.Text = I18n.T("Buscar") + ":";
                
                // CheckBox
                chkSoloActivos.Text = I18n.T("Solo ver activos");
                
                // Columnas DataGridView
                colCodigo.HeaderText = I18n.T("Código");
                colDescripcion.HeaderText = I18n.T("Descripción");
                colIVA.HeaderText = I18n.T("IVA");
                colEstado.HeaderText = I18n.T("Estado");
                colFechaAlta.HeaderText = I18n.T("Fecha Alta");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al aplicar traducciones: {ex.Message}");
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    I18n.T("Debe seleccionar un producto para editar"),
                    I18n.T("Validación"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            
            // Abrir formulario de edición
        }

        private void btnInhabilitar_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show(
                I18n.T("¿Está seguro de inhabilitar este producto?"),
                I18n.T("Confirmar eliminación"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    // Lógica de inhabilitación
                    MessageBox.Show(
                        I18n.T("Producto inhabilitado exitosamente"),
                        I18n.T("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        I18n.T("Error al inhabilitar producto") + ": " + ex.Message,
                        I18n.T("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}
```

## Resumen

Este sistema de traducción dinámica te permite:
- ✅ Cambiar el idioma de toda la aplicación desde la configuración
- ✅ Agregar nuevas traducciones fácilmente en el archivo de texto
- ✅ Usar traducciones en todos los formularios con un simple `I18n.T("clave")`
- ✅ Mantener el código limpio y fácil de mantener
- ✅ Soportar múltiples idiomas (actualmente español e inglés)

Para usar en cualquier formulario:
1. `using SistemaPresupuestario.Helpers;`
2. Crear método `AplicarTraducciones()`
3. Llamarlo en `Form_Load`
4. Usar `I18n.T("clave")` para traducir textos
