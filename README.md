# Sistema Presupuestario - ABM Usuarios

## Descripción
Sistema completo de gestión de usuarios con arquitectura multicapa en .NET Framework 4.8, implementando patrones de diseño como Repository, Unit of Work y Composite para el manejo de permisos jerárquicos.

## Arquitectura Implementada

### 1. Capa de Dominio (DomainModel)
- **Entidades principales**: Usuario, Familia, Patente
- **Patrón Composite**: Implementado en IComponentePermiso para manejo jerárquico de permisos
- **Control de concurrencia**: Usando RowVersion (timestamp)
- **Validaciones**: Detección de ciclos en jerarquías de familias

### 2. Capa de Acceso a Datos (DAL)
- **Entity Framework 6**: Configuraciones fluent para todas las entidades
- **Patrón Repository**: Implementado con repositorios especializados
- **Unit of Work**: Manejo transaccional de operaciones
- **Inicialización de datos**: Seed con usuarios, familias y patentes de ejemplo

### 3. Capa de Lógica de Negocio (BLL)
- **DTOs**: UsuarioDto, UsuarioEditDto, FamiliaDto, PatenteDto, PermisoEfectivoDto
- **AutoMapper**: Perfiles para mapeo automático entidad-DTO
- **Excepciones personalizadas**: DomainValidationException, ConcurrencyException, BusinessRuleException
- **Hashing de contraseñas**: SHA256 con salt y 10,000 iteraciones

### 4. Capa de Servicio (Service)
- **Fachadas de servicio**: IUsuarioService para operaciones de usuario
- **Coordinación de capas**: Orquesta BLL y DAL

### 5. Interfaz de Usuario (WinForms)
- **FrmUsuarios**: Lista principal con búsqueda y paginación
- **FrmUsuarioEdit**: Formulario de alta/edición con validaciones
- **Controles especializados**: TreeView para familias, CheckedListBox para permisos

## Características Implementadas

### Gestión de Usuarios
- ✅ CRUD completo de usuarios
- ✅ Validación de unicidad de nombre de usuario
- ✅ Hashing seguro de contraseñas
- ✅ Control de concurrencia con timestamps
- ✅ Búsqueda y filtrado
- ✅ Asignación de familias y patentes

### Sistema de Permisos Jerárquicos
- ✅ Familias pueden contener otras familias y patentes
- ✅ Cálculo de permisos efectivos (directos + heredados)
- ✅ Detección y prevención de ciclos
- ✅ Visualización clara de permisos directos vs heredados

### Seguridad
- ✅ Contraseñas hasheadas con SHA256 + salt
- ✅ Validación de fortaleza de contraseñas
- ✅ Control de concurrencia optimista
- ✅ Validación de entrada en todas las capas

### Base de Datos
- ✅ Estructura normalizada con relaciones many-to-many
- ✅ Índices únicos para integridad
- ✅ Datos de ejemplo precargados
- ✅ Inicialización automática

## Datos de Ejemplo Incluidos

### Usuarios Predefinidos
- **admin/admin123**: Administrador con todos los permisos
- **editor/editor123**: Editor con permisos de edición
- **lector/lector123**: Lector con permisos básicos

### Jerarquía de Familias
```
Administradores
├── Gestión de Usuarios
├── Reportes  
├── Configuración
└── Editores
    ├── Edición de Usuarios
    └── Lectores
        └── Lectura de Usuarios
```

## Instrucciones de Uso

### Requisitos
- .NET Framework 4.8
- SQL Server LocalDB (incluido con Visual Studio)
- Visual Studio 2019 o superior

### Instalación
1. Clonar el repositorio
2. Abrir `SistemaPresupuestario.sln` en Visual Studio
3. Compilar la solución (restaura paquetes NuGet automáticamente)
4. Ejecutar el proyecto `SistemaPresupuestario.UI`

### Primera Ejecución
La base de datos se crea automáticamente con datos de ejemplo al iniciar la aplicación por primera vez.

## Patrones de Diseño Implementados

### 1. Composite Pattern
- **Propósito**: Manejo uniforme de familias y patentes en jerarquías
- **Implementación**: IComponentePermiso con métodos polimórficos
- **Beneficio**: Cálculo recursivo de permisos efectivos

### 2. Repository Pattern
- **Propósito**: Abstracción del acceso a datos
- **Implementación**: Repositorios especializados por entidad
- **Beneficio**: Testabilidad y separación de responsabilidades

### 3. Unit of Work Pattern
- **Propósito**: Control transaccional de operaciones múltiples
- **Implementación**: IUnitOfWork con gestión de transacciones
- **Beneficio**: Consistencia de datos en operaciones complejas

### 4. DTO Pattern
- **Propósito**: Transferencia de datos entre capas
- **Implementación**: DTOs específicos para lectura y edición
- **Beneficio**: Versionado de API y validación estructurada

## Próximas Mejoras Sugeridas

1. **Logging**: Implementar ILogger para auditoría
2. **Cache**: Agregar cache para consultas frecuentes de permisos
3. **Validación avanzada**: Reglas de negocio más complejas
4. **Reportes**: Módulo de reportes de usuarios y permisos
5. **Configuración**: Sistema de configuración flexible
6. **Testing**: Tests unitarios e integración
7. **Localización**: Soporte multi-idioma

## Notas Técnicas

- Base de datos: SQL Server LocalDB por defecto
- ORM: Entity Framework 6.4.4
- Mapeo: AutoMapper 10.1.1
- Hash: SHA256 con 10,000 iteraciones
- Arquitectura: N-Layer con separación clara de responsabilidades