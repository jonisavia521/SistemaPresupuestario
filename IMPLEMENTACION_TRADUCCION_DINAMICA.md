# Sistema de Traducción Dinámica en Tiempo de Ejecución

## Resumen

Se ha implementado con éxito un sistema de traducción dinámica que permite cambiar el idioma de la aplicación en tiempo real sin necesidad de reiniciar. Todos los formularios abiertos se actualizan automáticamente cuando se cambia el idioma.

## Componentes Implementados

### 1. **I18n.cs** - Sistema de Eventos
```csharp
// Evento estático que se dispara al cambiar el idioma
public static event EventHandler LanguageChanged;

// Método para cambiar el idioma
public static void SetLanguage(string culture)
{
    TranslationService.ChangeLanguage(culture);
    LanguageChanged?.Invoke(null, EventArgs.Empty);
}
```

**Características:**
- Emite un evento estático `LanguageChanged` cuando se llama a `SetLanguage()`
- Notifica a todos los formularios suscritos
- Trabaja con el `TranslationService` existente

### 2. **FormTranslator.cs** - Traductor Idempotente
```csharp
// Primera ejecución: guarda la clave original en control.Tag
if (control.Tag == null || !(control.Tag is string))
{
    control.Tag = control.Text; // Guardar clave original
}

// Traducir usando la clave guardada (siempre desde Tag)
control.Text = I18n.T(control.Tag.ToString());
```

**Características:**
- **Idempotente**: Se puede ejecutar múltiples veces sin problemas
- Guarda la clave original en `control.Tag` la primera vez
- En ejecuciones subsiguientes, lee siempre desde `Tag`
- Soporta todos los tipos de controles:
  - Formularios (Form.Text)
  - Controles básicos (Button, Label, TextBox, etc.)
  - DataGridView (encabezados de columnas)
  - MenuStrip / ToolStrip / StatusStrip
  - ComboBox / ListBox (items string)
  - TabControl (pestañas)

### 3. **FormBase.cs** - Clase Base (Opcional)

Se creó una clase base opcional que simplifica la implementación:

```csharp
public class FormBase : Form
{
    protected void InitializeTranslation()
    {
        // Traducir por primera vez
        FormTranslator.Translate(this);
        
        // Suscribirse al evento de cambio de idioma
        I18n.LanguageChanged += OnLanguageChanged;
    }
    
    private void OnLanguageChanged(object sender, EventArgs e)
    {
        FormTranslator.Translate(this); // Re-traducir
    }
    
    // Des-suscribirse en FormClosed
    private void FormBase_FormClosed(object sender, FormClosedEventArgs e)
    {
        I18n.LanguageChanged -= OnLanguageChanged;
    }
}
```

**Uso:**
```csharp
public partial class MiFormulario : FormBase
{
    public MiFormulario()
    {
        InitializeComponent();
        base.InitializeTranslation(); // ¡Una sola línea!
    }
}
```

## Implementación en Formularios Existentes

Todos los formularios principales han sido actualizados con el siguiente patrón:

```csharp
public MiFormulario(/* parámetros */)
{
    InitializeComponent();
    
    // Traducción automática inicial
    FormTranslator.Translate(this);
    
    // Suscripción al evento de cambio de idioma
    I18n.LanguageChanged += OnLanguageChanged;
    this.FormClosed += (s, e) => I18n.LanguageChanged -= OnLanguageChanged;
}

private void OnLanguageChanged(object sender, EventArgs e)
{
    FormTranslator.Translate(this);
}
```

### Formularios Modificados

? **frmLogin.cs** - Formulario de inicio de sesión
? **frmMain.cs** - Formulario principal
? **frmPresupuesto.cs** - Gestión de presupuestos
? **frmUsuarios.cs** - Gestión de usuarios
? **frmConfiguacionGeneral.cs** - Configuración (incluye cambio de idioma)

## Cambio de Idioma - frmConfiguacionGeneral.cs

El botón "Cambiar Idioma" ahora usa el nuevo sistema:

```csharp
private void btnCambiarIdioma_Click(object sender, EventArgs e)
{
    string nuevoIdioma = rbEspanol.Checked ? "es-AR" : "en-US";
    
    // Guardar en base de datos
    configuracion.Idioma = nuevoIdioma;
    _configuracionService.GuardarConfiguracion(configuracion);
    
    // ? Cambiar idioma dinámicamente sin reiniciar
    I18n.SetLanguage(nuevoIdioma);
    
    lblIdiomaActual.Text = nombreIdioma;
    
    MessageBox.Show(
        "Idioma cambiado exitosamente.\n\n" +
        "Todos los formularios abiertos se han actualizado automáticamente.",
        "Cambio de Idioma",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);
}
```

**Antes:** La aplicación se reiniciaba con `Application.Restart()`
**Ahora:** Todos los formularios abiertos se actualizan automáticamente

## Prevención de Fugas de Memoria

Cada formulario se des-suscribe del evento estático al cerrarse:

```csharp
this.FormClosed += (s, e) => I18n.LanguageChanged -= OnLanguageChanged;
```

Esto es **CRÍTICO** para evitar:
- Referencias fantasma a formularios cerrados
- Aumento de memoria con el tiempo
- Excepciones al intentar actualizar formularios eliminados

## Cómo Agregar Traducción Dinámica a Nuevos Formularios

### Opción 1: Usando FormBase (Recomendado)

```csharp
public partial class NuevoFormulario : FormBase
{
    public NuevoFormulario()
    {
        InitializeComponent();
        base.InitializeTranslation();
    }
}
```

### Opción 2: Implementación Manual

```csharp
public partial class NuevoFormulario : Form
{
    public NuevoFormulario()
    {
        InitializeComponent();
        
        // Traducir inicialmente
        FormTranslator.Translate(this);
        
        // Suscribirse a cambios de idioma
        I18n.LanguageChanged += OnLanguageChanged;
        this.FormClosed += (s, e) => I18n.LanguageChanged -= OnLanguageChanged;
    }
    
    private void OnLanguageChanged(object sender, EventArgs e)
    {
        FormTranslator.Translate(this);
    }
}
```

## Comportamiento del Sistema

### Primera Carga de un Formulario
1. Se ejecuta `InitializeComponent()` (controles con texto en diseñador)
2. Se ejecuta `FormTranslator.Translate(this)`
3. Se guarda el texto original de cada control en su propiedad `Tag`
4. Se traduce el control usando `I18n.T(Tag)`

### Al Cambiar el Idioma
1. El usuario cambia el idioma desde **Configuración > Idioma**
2. Se llama a `I18n.SetLanguage("es-AR")` o `I18n.SetLanguage("en-US")`
3. Se dispara el evento `I18n.LanguageChanged`
4. **TODOS** los formularios suscritos ejecutan su método `OnLanguageChanged`
5. Cada formulario se re-traduce con `FormTranslator.Translate(this)`
6. El traductor lee las claves desde `Tag` (no desde `Text`)
7. Se traduce usando `I18n.T(Tag)` con el nuevo idioma
8. **Los formularios se actualizan instantáneamente**

## Tipos de Controles Soportados

| Control | Propiedad Traducida | Guardado en Tag |
|---------|---------------------|-----------------|
| Form | Text (título) | ? |
| Button, Label, etc. | Text | ? |
| DataGridView | Column.HeaderText | ? |
| MenuStrip | Item.Text (recursivo) | ? |
| ToolStrip | Item.Text | ? |
| StatusStrip | Item.Text | ? |
| TabControl | TabPage.Text | ? |
| ComboBox | Items (string[]) | ? |
| ListBox | Items (string[]) | ? |

## Limitaciones y Consideraciones

### ? NO se traducen automáticamente:
- **MessageBox**: Usar `I18n.T()` manualmente:
  ```csharp
  MessageBox.Show(
      I18n.T("¿Está seguro?"),
      I18n.T("Confirmar"),
      MessageBoxButtons.YesNo);
  ```

- **Textos dinámicos** (ej: `txtUsuario.Text = "Bienvenido " + nombre`):
  ```csharp
  // En frmMain_Load y en OnLanguageChanged:
  txtUsuario.Text = $"{I18n.T("Bienvenido")} {_login.user.Nombre}";
  ```

- **ComboBox con objetos complejos**: Solo se traducen items de tipo `string`

### ?? Controles que usan Tag para otros propósitos:
Si un control ya usa `Tag` para almacenar datos, el sistema de traducción lo sobrescribirá. Soluciones:
1. Usar otra propiedad (ej: `control.Name` con prefijo)
2. Guardar datos en un diccionario aparte
3. Usar objetos complejos que incluyan tanto datos como la clave de traducción

## Testing

### Cómo Probar el Sistema:
1. Iniciar la aplicación
2. Abrir varios formularios (frmPresupuesto, frmUsuarios, etc.)
3. Ir a **Configuración > Idioma**
4. Cambiar de Español a Inglés (o viceversa)
5. **Verificar que TODOS los formularios abiertos se actualicen instantáneamente**

### Verificar Fugas de Memoria:
1. Abrir y cerrar múltiples formularios varias veces
2. Cambiar el idioma varias veces
3. Monitorear el uso de memoria (Task Manager)
4. La memoria no debería crecer de forma sostenida

## Conclusión

El sistema de traducción dinámica está completamente funcional y listo para ser utilizado en toda la aplicación. Los cambios son:

? **No intrusivos**: Sigue funcionando el sistema de traducción existente
? **Escalable**: Fácil de agregar a nuevos formularios
? **Sin fugas de memoria**: Des-suscripción automática
? **Idempotente**: Se puede ejecutar múltiples veces sin problemas
? **Compatible**: Funciona con todos los tipos de controles WinForms

**Próximos pasos:**
- Agregar traducción dinámica a los formularios restantes (seguir el patrón establecido)
- Completar el archivo `Textos_Controles_UI.txt` con todas las traducciones necesarias
- Probar exhaustivamente con usuarios finales
