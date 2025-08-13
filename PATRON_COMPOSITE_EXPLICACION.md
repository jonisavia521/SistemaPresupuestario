# Patrón Composite - Sistema de Permisos

## ¿Qué es el Patrón Composite?

El patrón Composite es un patrón de diseño estructural que **permite tratar objetos individuales y composiciones de objetos de manera uniforme**. Es especialmente útil cuando necesitas trabajar con estructuras jerárquicas tipo árbol.

## Analogía Coloquial Simple

### 🗂️ **Sistema de Archivos de la Computadora**

Imagina el explorador de archivos de tu computadora:

- **CARPETAS**: Pueden contener archivos Y otras carpetas
- **ARCHIVOS**: Son elementos individuales que no contienen nada más
- **OPERACIONES**: Puedes copiar, mover, eliminar tanto carpetas como archivos de la misma manera

```
📁 Mis Documentos/
├── 📄 curriculum.pdf          (ARCHIVO - no tiene hijos)
├── 📄 carta_presentacion.docx  (ARCHIVO - no tiene hijos)
└── 📁 Proyectos/              (CARPETA - tiene hijos)
    ├── 📄 proyecto1.pdf       (ARCHIVO - no tiene hijos)
    ├── 📁 Imágenes/           (CARPETA - tiene hijos)
    │   ├── 📄 logo.png        (ARCHIVO - no tiene hijos)
    │   └── 📄 banner.jpg      (ARCHIVO - no tiene hijos)
    └── 📄 proyecto2.pdf       (ARCHIVO - no tiene hijos)
```

## Nuestro Sistema de Permisos

En nuestro sistema, aplicamos esta misma idea para organizar los permisos de usuario:

### 🏢 **Analogía: Llaves de un Edificio**

- **PATENTE** = Una llave específica que abre UNA puerta particular
- **FAMILIA** = Un llavero que contiene múltiples llaves (y puede contener otros llaveros)
- **USUARIO** = La persona que tiene el conjunto de llaveros y llaves

```
👤 Usuario: "Juan Pérez - Administrador"
├── 🗝️ PATENTE: "Login"                    (Llave individual)
├── 🎗️ FAMILIA: "Administración"           (Llavero principal)
│   ├── 🗝️ PATENTE: "CrearUsuarios"        (Llave específica)
│   ├── 🗝️ PATENTE: "EliminarUsuarios"     (Llave específica)
│   └── 🎗️ FAMILIA: "Configuración"        (Sub-llavero)
│       ├── 🗝️ PATENTE: "VerLogs"          (Llave específica)
│       └── 🗝️ PATENTE: "CambiarConfig"    (Llave específica)
└── 🎗️ FAMILIA: "Reportes"                 (Otro llavero)
    ├── 🗝️ PATENTE: "GenerarReporte"       (Llave específica)
    └── 🗝️ PATENTE: "ExportarReporte"      (Llave específica)
```

## Estructura del Patrón en Nuestro Código

### 🧩 Los Componentes

| Clase | Rol en el Patrón | Analogía | ¿Puede tener hijos? |
|-------|------------------|----------|-------------------|
| `Component` | **Component** | "Elemento del sistema de permisos" | - (es abstracta) |
| `Patente` | **Leaf** | "Llave individual" | ❌ No |
| `Familia` | **Composite** | "Llavero/Carpeta" | ✅ Sí |
| `Usuario` | **Client** | "Persona con llaves" | - (usa el patrón) |

### 🔧 Las Operaciones

```csharp
// OPERACIONES COMUNES (todas las clases las implementan)
Add(component)      // Agregar un elemento hijo
Remove(component)   // Quitar un elemento hijo  
ChildrenCount()     // Contar cuántos hijos tiene
Set(components)     // Establecer múltiples hijos

// COMPORTAMIENTO SEGÚN EL TIPO:
// 
// PATENTE (Leaf):
patente.Add(...)         // ❌ Lanza excepción
patente.ChildrenCount()  // ✅ Retorna 0 siempre

// FAMILIA (Composite):
familia.Add(...)         // ✅ Agrega a su lista interna
familia.ChildrenCount()  // ✅ Retorna cantidad de hijos
```

## Ejemplos Prácticos de Uso

### 📝 Ejemplo 1: Creando la Estructura

```csharp
// CREAR PATENTES INDIVIDUALES (Hojas)
var patenteLogin = new Patente 
{ 
    FormName = "frmLogin",
    MenuItemName = "mnuLogin" 
};

var patenteCrearUsuario = new Patente 
{ 
    FormName = "frmUsuarios",
    MenuItemName = "mnuCrearUsuario" 
};

var patenteReporte = new Patente 
{ 
    FormName = "frmReportes",
    MenuItemName = "mnuGenerarReporte" 
};

// CREAR FAMILIA (Composite)
var familiaAdmin = new Familia();
familiaAdmin.Nombre = "Administración";
familiaAdmin.Add(patenteCrearUsuario);

// CREAR USUARIO (Client)
var usuario = new Usuario();
usuario.Nombre = "Juan Pérez";
usuario.User = "jperez";

// ASIGNAR PERMISOS AL USUARIO
usuario.Permisos.Add(patenteLogin);      // Permiso individual
usuario.Permisos.Add(familiaAdmin);      // Grupo de permisos
usuario.Permisos.Add(patenteReporte);    // Otro permiso individual
```

### 🔍 Ejemplo 2: Recorriendo la Estructura

```csharp
// OBTENER TODOS LOS PERMISOS DEL USUARIO
var todasLasPatentes = usuario.GetPatentesAll();

// Resultado: Lista con todas las patentes, sin importar 
// si estaban directamente asignadas o dentro de familias:
// ["frmLogin", "frmUsuarios", "frmReportes"]

// EL MÉTODO INTERNAMENTE HACE ESTO:
foreach (var permiso in usuario.Permisos)
{
    if (permiso.ChildrenCount() == 0)  
    {
        // Es una PATENTE - procesar directamente
        Console.WriteLine($"Patente: {((Patente)permiso).FormName}");
    }
    else  
    {
        // Es una FAMILIA - recorrer sus hijos recursivamente
        var familia = (Familia)permiso;
        Console.WriteLine($"Familia: {familia.Nombre}");
        // ... llamada recursiva para procesar hijos
    }
}
```

### 🏗️ Ejemplo 3: Construyendo Jerarquías Complejas

```csharp
// ESCENARIO: Sistema de punto de venta con múltiples niveles

// === NIVEL 1: PATENTES BÁSICAS ===
var patenteLogin = new Patente { FormName = "frmLogin" };
var patenteFacturar = new Patente { FormName = "frmFacturacion" };
var patenteCobrar = new Patente { FormName = "frmCobranza" };

// === NIVEL 2: FAMILIAS ESPECIALIZADAS ===
var familiaVentas = new Familia();
familiaVentas.Nombre = "Ventas";
familiaVentas.Add(patenteFacturar);
familiaVentas.Add(patenteCobrar);

// === NIVEL 3: PATENTES ADMINISTRATIVAS ===
var patenteCrearUsuario = new Patente { FormName = "frmUsuarios" };
var patenteBackup = new Patente { FormName = "frmBackup" };

var familiaAdministracion = new Familia();
familiaAdministracion.Nombre = "Administración";
familiaAdministracion.Add(patenteCrearUsuario);
familiaAdministracion.Add(patenteBackup);

// === NIVEL 4: FAMILIA MASTER QUE INCLUYE OTRAS FAMILIAS ===
var familiaSupervisor = new Familia();
familiaSupervisor.Nombre = "Supervisor General";
familiaSupervisor.Add(familiaVentas);        // ¡Familia dentro de familia!
familiaSupervisor.Add(familiaAdministracion); // ¡Otra familia!

// === USUARIO FINAL ===
var supervisor = new Usuario();
supervisor.Nombre = "María González";
supervisor.Permisos.Add(patenteLogin);        // Permiso individual
supervisor.Permisos.Add(familiaSupervisor);   // ¡TODO el árbol de permisos!

// RESULTADO: El supervisor tendrá acceso a:
// - Login (directo)
// - Facturación (via Supervisor -> Ventas)  
// - Cobranza (via Supervisor -> Ventas)
// - Crear Usuario (via Supervisor -> Administración)
// - Backup (via Supervisor -> Administración)
```

## Ventajas del Patrón en Nuestro Sistema

### ✅ **1. Simplicidad de Uso**
```csharp
// EL USUARIO NO NECESITA SABER SI ES PATENTE O FAMILIA
foreach (var permiso in usuario.Permisos)
{
    permiso.Add(nuevoPermiso); // Funciona igual para ambos tipos
}
```

### ✅ **2. Flexibilidad de Estructura**
```csharp
// PUEDES MEZCLAR PERMISOS INDIVIDUALES Y GRUPOS
usuario.Permisos.Add(patenteLogin);         // Individual
usuario.Permisos.Add(familiaAdministracion); // Grupo
usuario.Permisos.Add(patenteEspecial);      // Individual otra vez
```

### ✅ **3. Escalabilidad**
```csharp
// AGREGAR NUEVOS NIVELES ES TRIVIAL
var departamento = new Familia();
departamento.Add(familiaVentas);
departamento.Add(familiaMarketing);
departamento.Add(familiaContabilidad);
// ¡Infinitos niveles de jerarquía!
```

### ✅ **4. Reutilización**
```csharp
// LA MISMA FAMILIA SE PUEDE ASIGNAR A MÚLTIPLES USUARIOS
usuarioVendedor1.Permisos.Add(familiaVentas);
usuarioVendedor2.Permisos.Add(familiaVentas);
usuarioVendedor3.Permisos.Add(familiaVentas);
```

## Casos de Uso Reales

### 🎯 **1. Construcción de Menús Dinámicos**
```csharp
// El sistema puede construir el menú de cada usuario
var patentes = usuario.GetPatentesAll();
foreach (var patente in patentes)
{
    menuPrincipal.AgregarOpcion(patente.MenuItemName, patente.FormName);
}
```

### 🎯 **2. Control de Acceso a Formularios**
```csharp
public bool PuedeAcceder(string formulario)
{
    var patentes = usuario.GetPatentesAll();
    return patentes.Any(p => p.FormName == formulario);
}
```

### 🎯 **3. Auditoría de Permisos**
```csharp
public void AuditarPermisos(Usuario usuario)
{
    Console.WriteLine($"Permisos de {usuario.Nombre}:");
    usuario.RecorrerComposite(...); // Muestra toda la estructura
}
```

## Comparación: Con vs Sin Patrón Composite

### ❌ **SIN Patrón Composite (Problemático)**
```csharp
public class Usuario
{
    public List<Patente> PermisosIndividuales { get; set; }
    public List<GrupoPermisos> GruposDePermisos { get; set; }
    
    public List<Patente> GetTodosLosPermisos()
    {
        var resultado = new List<Patente>();
        
        // Procesar permisos individuales
        resultado.AddRange(PermisosIndividuales);
        
        // Procesar grupos (¿y si hay subgrupos?)
        foreach (var grupo in GruposDePermisos)
        {
            resultado.AddRange(grupo.Patentes);
            // ¿Y si el grupo tiene subgrupos? ¿Más código?
        }
        
        return resultado;
    }
}
```

### ✅ **CON Patrón Composite (Elegante)**
```csharp
public class Usuario  
{
    public List<Component> Permisos { get; set; } // ¡Una sola lista!
    
    public List<Patente> GetTodosLosPermisos()
    {
        var resultado = new List<Patente>();
        RecorrerComposite(resultado, Permisos, ""); // ¡Un solo método recursivo!
        return resultado;
    }
}
```

## Preguntas Frecuentes

### ❓ **¿Por qué Patente lanza excepciones en Add/Remove?**
**R:** Porque es un elemento "hoja" (leaf) que por definición no puede contener hijos. Esto mantiene la integridad del patrón y evita errores de programación.

### ❓ **¿Puedo crear jerarquías infinitas?**
**R:** Técnicamente sí, pero debes validar referencias circulares (Familia A contiene Familia B, y Familia B contiene Familia A).

### ❓ **¿Qué pasa si asigno la misma patente múltiples veces?**
**R:** El método `GetPatentesAll()` usa `patentes.Exists()` para evitar duplicados.

### ❓ **¿Puedo agregar nuevos tipos de componentes?**
**R:** ¡Sí! Solo crea una nueva clase que herede de `Component` e implemente los métodos abstractos.

## Resumen

El patrón Composite en nuestro sistema de permisos nos permite:

1. **Tratar uniformemente** permisos individuales y grupos de permisos
2. **Crear jerarquías flexibles** de cualquier profundidad  
3. **Simplificar el código cliente** que usa los permisos
4. **Facilitar el mantenimiento** y extensión del sistema
5. **Reutilizar** configuraciones de permisos entre usuarios

Es como tener un sistema de archivos para permisos: simple, flexible y poderoso. 🚀