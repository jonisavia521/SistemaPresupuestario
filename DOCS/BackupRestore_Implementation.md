# Funcionalidad de Backup/Restore - Sistema Presupuestario

## Descripción General

Esta implementación proporciona funcionalidad completa de Backup y Restore para la base de datos SQL Server del Sistema Presupuestario, siguiendo una arquitectura N-Capas estricta y los principios SOLID.

## Arquitectura

La solución sigue una separación estricta de responsabilidades en 3 capas:

```
???????????????????????????????????????????
?   UI (Presentación)                     ?
?   SistemaPresupuestario\Configuracion   ?
?   ??? frmBackupRestore.cs               ? ? Formulario "tonto" (sin lógica)
???????????????????????????????????????????
              ? Llama solo a ?
???????????????????????????????????????????
?   BLL (Lógica de Negocio)               ?
?   Services\BLL                          ?
?   ??? BackupRestoreService.cs           ? ? Clase "inteligente" (orquestación)
?   ??? Contracts\                        ?
?       ??? IBackupRestoreService.cs      ?
???????????????????????????????????????????
              ? Llama solo a ?
???????????????????????????????????????????
?   DAL (Acceso a Datos)                  ?
?   Services\DAL                          ?
?   ??? BackupRepository.cs               ? ? Solo ejecuta SQL (sin lógica)
???????????????????????????????????????????
```

## Archivos Creados

### 1. Capa de Datos (DAL)
**Archivo:** `Services\DAL\BackupRepository.cs`

Responsabilidades:
- Ejecutar comandos SQL de BACKUP y RESTORE
- Gestionar conexiones a SQL Server (base de datos y master)
- Insertar registros en el historial
- Obtener historial de backups

Métodos públicos:
- `void ExecuteBackup(string rutaArchivo)` - Ejecuta BACKUP DATABASE
- `void ExecuteRestore(string rutaArchivo)` - Ejecuta RESTORE DATABASE con SINGLE_USER
- `void InsertHistorial(string ruta, string usuario, string estado, string error)` - Registra operación
- `DataTable GetHistorial()` - Obtiene historial ordenado por fecha DESC

### 2. Capa de Servicio (BLL)

#### Interfaz
**Archivo:** `Services\BLL\Contracts\IBackupRestoreService.cs`

Define el contrato del servicio con métodos asíncronos:
- `Task CrearBackupAsync(string rutaArchivo, string usuarioApp)`
- `Task RestaurarBackupAsync(string rutaArchivo)`
- `DataTable ObtenerHistorial()`

#### Implementación
**Archivo:** `Services\BLL\BackupRestoreService.cs`

Responsabilidades:
- Orquestar operaciones asíncronas (async/await)
- Manejar errores con try-catch-finally
- Registrar en historial (éxito o fallo) de forma síncrona en el finally
- Relanzar excepciones después de registrarlas

Características clave:
- `CrearBackupAsync`: Ejecuta backup asíncrono y SIEMPRE registra en historial (en finally)
- `RestaurarBackupAsync`: Ejecuta restore asíncrono con manejo de errores
- Uso de `Task.Run()` para convertir operaciones síncronas en asíncronas

### 3. Capa de Presentación (UI)

**Archivos:**
- `SistemaPresupuestario\Configuracion\frmBackupRestore.cs` (Code-behind)
- `SistemaPresupuestario\Configuracion\frmBackupRestore.Designer.cs` (Diseñador)
- `SistemaPresupuestario\Configuracion\frmBackupRestore.resx` (Recursos)

Controles:
- `btnCrearBackup` - Botón para crear backup
- `btnRestaurar` - Botón para restaurar backup
- `dgvHistorial` - DataGridView con historial
- `progressBar` - ProgressBar estilo Marquee
- `lblEstado` - Label para mostrar mensajes de estado

Características:
- Formulario "tonto" sin lógica de negocio
- No contiene `using System.Data.SqlClient`
- Usa `SaveFileDialog` y `OpenFileDialog` para selección de archivos
- Método helper `SetTrabajando(bool, string)` para UI feedback
- Manejo completo con async/await
- Advertencia crítica antes de restaurar
- Reinicia aplicación después de restauración exitosa

### 4. Base de Datos

**Archivo:** `DOCS\SQL\CreateTable_HistorialBackups.sql`

Crea la tabla `dbo.HistorialBackups` con:
- `ID` (INT IDENTITY) - PK
- `FechaHora` (DATETIME) - DEFAULT GETDATE()
- `RutaArchivo` (NVARCHAR(500))
- `Estado` (NVARCHAR(50)) - 'Exitoso' o 'Fallido'
- `MensajeError` (NVARCHAR(1000)) - NULL si exitoso
- `UsuarioApp` (NVARCHAR(100)) - Usuario de la aplicación

Índices:
- Índice en `FechaHora DESC` para consultas de historial
- Índice en `Estado` para filtros

## Instalación

### Paso 1: Ejecutar el Script SQL

Ejecutar el script en SQL Server Management Studio:
```sql
-- Conectarse a la base de datos SistemaPresupuestario
USE [SistemaPresupuestario]
GO

-- Ejecutar el script completo
-- DOCS\SQL\CreateTable_HistorialBackups.sql
```

### Paso 2: Verificar Connection Strings

Asegurarse de que `App.config` tenga la connection string correcta:
```xml
<connectionStrings>
    <add name="SistemaPresupuestario" 
         connectionString="data source=SERVER\INSTANCE;initial catalog=SistemaPresupuestario;integrated security=True;MultipleActiveResultSets=True" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Paso 3: Agregar al Menú Principal (Opcional)

En `frmMain.cs`, agregar un ítem de menú para abrir el formulario:

```csharp
private void tsBackupRestore_Click(object sender, EventArgs e)
{
    var formAbierto = Application.OpenForms.OfType<frmBackupRestore>()
        .FirstOrDefault(f => !f.IsDisposed);

    if (formAbierto != null)
    {
        formAbierto.BringToFront();
    }
    else
    {
        var hijo = new frmBackupRestore();
        hijo.MdiParent = this;
        hijo.Show();
    }
}
```

## Uso

### Crear un Backup

1. Abrir el formulario "Backup y Restore"
2. Hacer clic en "Crear Backup"
3. Seleccionar ubicación y nombre del archivo (.bak)
4. Esperar a que se complete la operación
5. Verificar en el historial que el estado sea "Exitoso"

### Restaurar un Backup

?? **ADVERTENCIA CRÍTICA**: Esta operación reemplaza TODOS los datos actuales.

1. Abrir el formulario "Backup y Restore"
2. Hacer clic en "Restaurar Backup"
3. Leer y confirmar la advertencia crítica
4. Seleccionar el archivo de backup (.bak)
5. Esperar a que se complete la operación
6. La aplicación se reiniciará automáticamente

## Características Técnicas

### Manejo de Transacciones
- **BACKUP**: Usa `WITH FORMAT, INIT, COMPRESSION`
- **RESTORE**: Usa `WITH REPLACE` y gestión de `SINGLE_USER`/`MULTI_USER`
- **Timeout**: 3600 segundos (1 hora) para operaciones largas

### Seguridad
- Connection strings desde App.config
- Validación de parámetros en todos los métodos
- Manejo de excepciones en todas las capas
- Registro de errores en historial

### Experiencia de Usuario
- ProgressBar con estilo Marquee durante operaciones
- Mensajes de estado en Label
- Deshabilitar botones durante operaciones
- Advertencia crítica antes de restaurar
- Actualización automática del historial

### Asincronía
- Operaciones asíncronas con `async/await`
- Uso de `Task.Run()` para operaciones de I/O largas
- UI responsive durante operaciones

## Pruebas

### Test 1: Crear Backup Exitoso
```
1. Crear backup en ubicación con permisos de escritura
2. Verificar que se crea el archivo .bak
3. Verificar registro en historial con Estado = "Exitoso"
```

### Test 2: Crear Backup con Error
```
1. Crear backup en ubicación sin permisos
2. Verificar mensaje de error en UI
3. Verificar registro en historial con Estado = "Fallido" y MensajeError
```

### Test 3: Restaurar Backup Exitoso
```
1. Crear un backup de prueba
2. Modificar algunos datos en la BD
3. Restaurar el backup
4. Verificar que los datos volvieron al estado del backup
5. Verificar que la aplicación se reinició
```

### Test 4: Historial
```
1. Realizar varias operaciones (exitosas y fallidas)
2. Verificar que todas aparecen en el historial
3. Verificar orden descendente por fecha
```

## Troubleshooting

### Error: "Cannot open backup device"
**Solución**: Verificar que la ruta del archivo sea accesible por la cuenta de servicio de SQL Server.

### Error: "Exclusive access could not be obtained"
**Solución**: Cerrar todas las conexiones activas a la base de datos antes de restaurar.

### Error: "Cannot find the object HistorialBackups"
**Solución**: Ejecutar el script SQL de creación de tabla.

### Error: Connection string not found
**Solución**: Verificar que App.config tenga la connection string "SistemaPresupuestario".

## Mejoras Futuras (Opcional)

1. **Compresión Mejorada**: Agregar opción para diferentes niveles de compresión
2. **Backup Diferencial**: Implementar backups diferenciales
3. **Backup Automático**: Agregar scheduler para backups automáticos
4. **Notificaciones**: Enviar emails al completar operaciones
5. **Verificación de Integridad**: Ejecutar `RESTORE VERIFYONLY` antes de restaurar
6. **Múltiples Bases de Datos**: Soporte para backup de múltiples BDs
7. **Azure Blob Storage**: Soporte para guardar backups en la nube

## Notas de Seguridad

1. **Permisos SQL**: La cuenta de SQL Server debe tener permisos de `BACKUP DATABASE` y `RESTORE DATABASE`
2. **Permisos de Archivo**: La cuenta de servicio debe tener permisos de lectura/escritura en las rutas de backup
3. **Connection String**: Nunca incluir passwords en plain text en repositorios públicos
4. **Auditoría**: Todos los backups y restores se registran con usuario y fecha/hora

## Soporte

Para consultas o reportar problemas, contactar al equipo de desarrollo del Sistema Presupuestario.

---

**Versión**: 1.0  
**Fecha**: 2024  
**Arquitecto**: Sistema Presupuestario Development Team
