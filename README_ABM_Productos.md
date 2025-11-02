# ABM de Productos - Sistema Presupuestario

## Descripción

Este documento describe la implementación completa del ABM (Alta, Baja, Modificación) de Productos en el Sistema Presupuestario, siguiendo la arquitectura en capas establecida y las buenas prácticas del proyecto.

## Estructura de Archivos Creados/Modificados

### 1. Capa de Dominio (DomainModel)

#### Entidades
- **DomainModel\Domain\ProductoDM.cs**
  - Entidad de dominio pura que representa un Producto
  - Contiene las propiedades básicas: ID, Codigo, Descripcion, Inhabilitado, FechaAlta, UsuarioAlta
  - Incluye el método `ValidarNegocio()` con las reglas de negocio

#### Contratos
- **DomainModel\Contract\IProductoRepository.cs**
  - Interface del repositorio de productos
  - Define los métodos: GetByCodigoAsync, GetActivosAsync, ExisteCodigoAsync

### 2. Capa de Acceso a Datos (DAL)

#### Repositorios
- **DAL\Implementation\Repository\ProductoRepository.cs**
  - Implementación concreta del repositorio
  - Maneja el mapeo entre entidades EF y entidades de dominio
  - Operaciones CRUD completas
  - Eliminación lógica (inhabilitación en lugar de borrado físico)

#### Configuración
- **DAL\DependencyContainer.cs** (modificado)
  - Registro del `IProductoRepository` y `ProductoRepository`

### 3. Capa de Lógica de Negocio (BLL)

#### DTOs
- **BLL\DTOs\ProductoDTO.cs**
  - Data Transfer Object con validaciones de entrada (DataAnnotations)
  - Propiedades: Id, Codigo, Descripcion, Inhabilitado, FechaAlta, UsuarioAlta
  - Propiedad calculada `EstadoTexto` para la UI

#### Servicios
- **BLL\Services\ProductoService.cs**
  - Servicio que coordina operaciones entre UI y DAL
  - Métodos: GetAllAsync, GetActivosAsync, GetByIdAsync, GetByCodigoAsync, AddAsync, UpdateAsync, DeleteAsync, ExisteCodigoAsync
  - Aplica validaciones de negocio antes de persistir

#### Contratos
- **BLL\Contracts\IProductoService.cs**
  - Interface del servicio de productos

#### Mappers
- **BLL\Mappers\ProductoMappingProfile.cs**
  - Perfil de AutoMapper para transformación entre ProductoDM y ProductoDTO

#### Configuración
- **BLL\DependencyContainer.cs** (modificado)
  - Registro del `IProductoService`, `ProductoService` y `ProductoMappingProfile`

### 4. Capa de Presentación (UI - SistemaPresupuestario)

#### Formularios
- **SistemaPresupuestario\Maestros\Productos\frmProductos.cs**
  - Formulario principal de listado de productos
  - Grilla con columnas: Código, Descripción, Estado, Fecha Alta
  - Funciones: Búsqueda en tiempo real, filtro por activos/inactivos
  - Botones: Nuevo, Editar, Inhabilitar, Cerrar

- **SistemaPresupuestario\Maestros\Productos\frmProductos.Designer.cs**
  - Diseño del formulario de listado

- **SistemaPresupuestario\Maestros\Productos\frmProductoAlta.cs**
  - Formulario de alta/edición de productos
  - Validación con ErrorProvider (no MessageBox)
  - Campos: Código, Descripción, Inhabilitado
  - Modo nuevo: checkbox inhabilitado oculto
  - Modo edición: código en solo lectura

- **SistemaPresupuestario\Maestros\Productos\frmProductoAlta.Designer.cs**
  - Diseño del formulario de alta/edición

#### Configuración
- **SistemaPresupuestario\Program.cs** (modificado)
  - Registro de `frmProductos` en el contenedor de dependencias

- **SistemaPresupuestario\frmMain.cs** (modificado)
  - Método `tsProducto_Click()` para abrir el formulario de productos

- **SistemaPresupuestario\frmMain.Designer.cs** (modificado)
  - Evento Click asignado al menú Productos

### 5. Base de Datos

- **Database\ScriptProducto.sql**
  - Script SQL para crear la tabla Producto
  - Índice único en el campo Codigo
  - Datos de ejemplo para pruebas

## Características Implementadas

### Funcionalidad Básica
- ? Listado de productos con grilla
- ? Alta de productos nuevos
- ? Edición de productos existentes
- ? Eliminación lógica (inhabilitación)
- ? Búsqueda en tiempo real
- ? Filtro por activos/inactivos

### Validaciones
- ? Validación de campos obligatorios (Código)
- ? Validación de longitud máxima de campos
- ? Validación de código único
- ? Validación de fecha de alta
- ? Validaciones de negocio en la entidad de dominio
- ? Uso de ErrorProvider para mostrar errores (no MessageBox)

### Arquitectura
- ? Separación en capas (Domain, DAL, BLL, UI)
- ? Inyección de dependencias
- ? Patrón Repository
- ? Unit of Work para transacciones
- ? AutoMapper para mapeo de entidades
- ? DTOs para transferencia de datos

### Buenas Prácticas
- ? Código siguiendo principios SOLID
- ? Manejo de excepciones centralizado
- ? Async/Await en operaciones de datos
- ? Eliminación lógica en lugar de física
- ? Validaciones en múltiples capas
- ? Código documentado con comentarios XML

## Cómo Usar

### 1. Ejecutar el Script de Base de Datos
```sql
-- Ejecutar en SQL Server Management Studio
-- Archivo: Database\ScriptProducto.sql
```

### 2. Acceder al ABM desde el Sistema
1. Iniciar sesión en el sistema
2. Ir al menú **Maestros ? Productos**
3. Se abrirá el formulario de gestión de productos

### 3. Operaciones Disponibles

#### Nuevo Producto
1. Clic en botón **Nuevo**
2. Ingresar código (obligatorio, máx. 50 caracteres)
3. Ingresar descripción (opcional, máx. 50 caracteres)
4. Clic en **Aceptar** para guardar

#### Editar Producto
1. Seleccionar un producto de la grilla
2. Clic en botón **Editar**
3. Modificar los datos (el código no se puede cambiar)
4. Clic en **Aceptar** para guardar

#### Inhabilitar Producto
1. Seleccionar un producto activo de la grilla
2. Clic en botón **Inhabilitar**
3. Confirmar la operación
4. El producto quedará inhabilitado (no se elimina físicamente)

#### Buscar Productos
- Escribir en el campo de búsqueda (busca por código o descripción)
- La grilla se actualiza en tiempo real

#### Filtrar por Estado
- Activar/desactivar el checkbox "Solo ver activos"
- La grilla muestra solo productos activos o todos según el filtro

## Reglas de Negocio

1. **Código del Producto**
   - Es obligatorio
   - Máximo 50 caracteres
   - Debe ser único en el sistema
   - No se puede modificar una vez creado

2. **Descripción**
   - Es opcional
   - Máximo 50 caracteres

3. **Fecha de Alta**
   - Se asigna automáticamente al crear el producto
   - No puede ser una fecha futura
   - Es inmutable

4. **Eliminación**
   - Los productos no se eliminan físicamente
   - Se marcan como inhabilitados
   - Los productos inhabilitados siguen apareciendo en las relaciones existentes

5. **Usuario Alta**
   - Se asigna automáticamente (actualmente hardcodeado como 1)
   - TODO: Implementar obtención desde contexto de usuario actual

## Mejoras Futuras Sugeridas

1. **Campos Adicionales**
   - Precio
   - Stock
   - Categoría
   - Proveedor
   - Código de barras
   - Imagen del producto

2. **Funcionalidades**
   - Importación/Exportación de productos desde Excel
   - Historial de cambios
   - Reactivación de productos inhabilitados
   - Búsqueda avanzada con múltiples criterios
   - Ordenamiento por columnas

3. **Reportes**
   - Listado de productos
   - Productos más vendidos
   - Stock bajo
   - Productos sin movimiento

4. **Integración**
   - Vincular con módulo de presupuestos
   - Vincular con módulo de facturación
   - Sincronización con sistema externo

## Notas Técnicas

- La entidad `Producto` en EF ya existía, solo se agregaron campos básicos
- El campo `UsuarioAlta` es de tipo `int` por compatibilidad con la BD existente
- Se utiliza `Guid` como tipo de ID para todas las entidades
- La eliminación es lógica mediante el campo `Inhabilitado`
- Todos los métodos de acceso a datos son asíncronos

## Validación de Compilación

? El proyecto compila sin errores
? Todas las dependencias están registradas correctamente
? Los formularios están integrados en el menú principal

---

**Fecha de Implementación:** 2024
**Versión:** 1.0
**Estado:** Completo y funcional
