# ABM de Clientes - Guía de Implementación

## ?? Resumen

Se ha implementado un ABM completo de Clientes siguiendo las buenas prácticas de arquitectura en capas establecidas en el proyecto.

## ??? Arquitectura Implementada

### 1. **DomainModel** (Capa de Dominio - NÚCLEO)
- ? `ClienteDM.cs`: Entidad de dominio con toda la lógica de negocio y validaciones
- ? `IClienteRepository.cs`: Interfaz del repositorio

**Características**:
- Validación de CUIT/CUIL con algoritmo de dígito verificador
- Validación de tipos de documento (DNI, CUIT, CUIL)
- Reglas de negocio encapsuladas en la propia entidad
- Constructores separados para creación y carga desde BD

### 2. **BLL** (Capa de Lógica de Negocio)
- ? `ClienteDTO.cs`: DTO para transferencia de datos con DataAnnotations
- ? `IClienteService.cs`: Interfaz del servicio de negocio
- ? `ClienteService.cs`: Implementación del servicio con lógica de negocio
- ? `ClienteMappingProfile.cs`: Perfil de AutoMapper
- ? `DependencyContainer.cs`: Registro de servicios y AutoMapper

**Responsabilidades**:
- Mapeo entre DTOs y entidades de dominio
- Validaciones adicionales antes de persistir
- Manejo de transacciones a través del UnitOfWork

### 3. **DAL** (Capa de Acceso a Datos)
- ? `ClienteRepository.cs`: Implementación del repositorio con Entity Framework
- ? `Cliente.cs` (EF): Entidad de Entity Framework actualizada con nuevos campos
- ? `SistemaPresupuestario.cs` (DbContext): Configuración de Fluent API
- ? `DependencyContainer.cs`: Registro del repositorio

**Características**:
- Mapeo entre entidades EF y entidades de dominio
- Consultas asíncronas con Entity Framework 6
- Eliminación lógica (campo Activo)

### 4. **UI** (SistemaPresupuestario)
- ? `frmClientes.cs` y `.Designer.cs`: Formulario de listado con filtros
- ? `frmClienteAlta.cs` y `.Designer.cs`: Formulario de alta/edición
- ? `Program.cs`: Registro de formularios en IoC
- ? `frmMain.cs` y `.Designer.cs`: Menú actualizado

**Características**:
- Búsqueda en tiempo real
- Filtro por clientes activos
- Validaciones en tiempo de escritura (KeyPress)
- Gestión de instancias MDI

### 5. **IoC** (Orquestador)
- ? Registro automático de todas las dependencias

## ?? Campos Implementados

| Campo | Tipo | Descripción | Validación |
|-------|------|-------------|------------|
| CodigoCliente | string(20) | Código único del cliente | Obligatorio, alfanumérico con guiones |
| RazonSocial | string(200) | Nombre/razón social | Obligatorio, mín. 3 caracteres |
| TipoDocumento | string | DNI\|CUIT\|CUIL | Obligatorio, lista fija |
| NumeroDocumento | string(11) | Número del documento | Validación específica por tipo |
| CodigoVendedor | string(2) | Código del vendedor asignado | Obligatorio, 2 dígitos numéricos |
| TipoIva | string(50) | Categoría de IVA | Lista predefinida de 5 opciones |
| CondicionPago | string(2) | Condición de pago | 2 dígitos numéricos |
| Email | string(100) | Email del cliente | Formato email válido (opcional) |
| Telefono | string(20) | Teléfono de contacto | Opcional |
| Direccion | string(200) | Dirección principal | Opcional |
| Activo | bool | Estado del cliente | Obligatorio, default: true |
| FechaAlta | DateTime | Fecha de creación | Obligatorio, automático |
| FechaModificacion | DateTime? | Última modificación | Automático |

## ?? Configuración Necesaria

### 1. **Referencias del Proyecto**

Asegúrate que el proyecto **SistemaPresupuestario** tenga estas referencias:

```xml
<ItemGroup>
  <ProjectReference Include="..\BLL\BLL.csproj" />
  <ProjectReference Include="..\Services\Services.csproj" />
  <ProjectReference Include="..\IoC\IoC.csproj" />
</ItemGroup>
```

### 2. **Base de Datos**

Ejecuta el script SQL incluido en `/Database/ScriptCliente.sql` para agregar los campos necesarios a la tabla Cliente:

```sql
-- Agregar campos: CodigoVendedor, TipoIva, CondicionPago, Email, Telefono, Activo, FechaAlta, FechaModificacion
```

### 3. **Paquetes NuGet**

Asegúrate que el proyecto **BLL** tenga AutoMapper:

```
PM> Install-Package AutoMapper.Extensions.Microsoft.DependencyInjection
```

## ?? Cómo Usar

### 1. **Desde el Menú Principal**

```
Maestros ? Clientes
```

### 2. **Operaciones Disponibles**

- **Nuevo**: Abre el formulario para crear un nuevo cliente
- **Editar**: Abre el formulario para editar el cliente seleccionado
- **Desactivar**: Desactiva el cliente seleccionado (eliminación lógica)
- **Reactivar**: Reactiva un cliente previamente desactivado
- **Buscar**: Filtra por código, razón social o documento
- **Solo ver activos**: Filtra para mostrar solo clientes activos

### 3. **Validaciones Automáticas**

- CUIT/CUIL: Valida el dígito verificador
- DNI: 7-8 dígitos
- Código Vendedor: Exactamente 2 dígitos
- Condición Pago: Exactamente 2 dígitos
- Email: Formato email válido

## ?? Flujo de Datos

### Escritura (Crear/Actualizar)
```
UI (frmClienteAlta)
  ? ClienteDTO
    ? BLL (ClienteService)
      ? ClienteDM (validaciones de negocio)
        ? DAL (ClienteRepository)
          ? EF Cliente
            ? Base de Datos
```

### Lectura (Consultar)
```
Base de Datos
  ? EF Cliente
    ? DAL (ClienteRepository)
      ? ClienteDM
        ? BLL (ClienteService) con AutoMapper
          ? ClienteDTO
            ? UI (frmClientes)
```

## ?? Principios Aplicados

### ? Separación de Responsabilidades
- **UI**: Solo captura y muestra datos
- **BLL**: Lógica de negocio y transformaciones
- **DomainModel**: Reglas de negocio y validaciones
- **DAL**: Persistencia de datos

### ? Dependency Inversion
- Todas las dependencias apuntan hacia las abstracciones (interfaces)
- El IoC resuelve las dependencias en tiempo de ejecución

### ? Single Responsibility
- Cada clase tiene una única responsabilidad
- Las validaciones están en el dominio
- El mapeo está aislado en AutoMapper

### ? Don't Repeat Yourself (DRY)
- Reutilización de componentes existentes (Repository<T>, UnitOfWork)
- Mapeo automático con AutoMapper

## ?? Solución de Problemas

### Error: "No se encontró la referencia a BLL"
**Solución**: Agregar referencia al proyecto BLL desde SistemaPresupuestario

### Error: "AutoMapper no registrado"
**Solución**: Verificar que `BLL/DependencyContainer.cs` tenga:
```csharp
services.AddAutoMapper(typeof(ClienteMappingProfile));
```

### Error: "Columnas no existen en la BD"
**Solución**: Ejecutar el script SQL `/Database/ScriptCliente.sql`

### Error: "CS0104 - Ambigüedad entre Cliente"
**Solución**: El repositorio usa correctamente `DomainModel.Domain.ClienteDM` y `DAL.Implementation.EntityFramework.Cliente`

## ?? Notas Adicionales

- El código del cliente se genera automáticamente en el formato `CLI-yyyyMMddHHmmss`
- El campo Código no es editable una vez creado el cliente
- La eliminación es lógica (campo Activo), no se borran registros físicamente
- Los campos Email, Teléfono y Dirección son opcionales
- El sistema valida duplicados de código y documento antes de guardar

## ?? Mejoras Futuras Sugeridas

1. **Auditoría**: Agregar usuario que creó/modificó el registro
2. **Historial**: Tabla de auditoría para cambios en clientes
3. **Importación**: Funcionalidad para importar clientes desde Excel
4. **Validación AFIP**: Consulta online para validar CUIT
5. **Geo codificación**: Validación de dirección con Google Maps API
6. **Reportes**: Crystal Reports o similar para listados

## ?? Soporte

Para cualquier duda sobre la implementación, consultar:
- Arquitectura general: `/contexto general.txt`
- Patrón Composite (permisos): `/Services/DomainModel/Security/Composite/Usuario.cs`
- Patrón Repository: `/DAL/Infrastructure/Repository.cs`
