# Auditoría de Dependencias NuGet - Sistema Presupuestario

## Resumen Ejecutivo

Esta auditoría analiza una solución .NET Framework 4.7.2 compuesta por 5 proyectos con **61 paquetes NuGet** identificando **8 conflictos críticos** y **15 paquetes no utilizados**.

### Hallazgos Principales

- ✅ **Target Framework**: Todos los proyectos correctamente configurados para .NET Framework 4.7.2
- ⚠️ **Proyecto Faltante**: Referencia a proyecto "Controller" inexistente en Program.cs
- 🔴 **Conflictos de Versión**: Microsoft.Bcl.AsyncInterfaces (6.0.0 vs 8.0.0)
- 🔴 **Paquetes Incompatibles**: 12 paquetes de las series 8.x incompatibles con net472
- 🟡 **Paquetes Azure**: Azure.Core y Azure.Identity no utilizados (HighConfidenceUnused)

## Decisiones de Compatibilidad

### Paquetes Mantenidos (EF Core 3.1 Stack)
- **Microsoft.EntityFrameworkCore 3.1.29** → **3.1.32** (último parche de seguridad)
- **Microsoft.Extensions.DependencyInjection 3.1.29** (compatible con net472)
- **AutoMapper 7.0.1** → **10.1.1** (última versión compatible net472)

### Paquetes Problemáticos - Series 8.x

**Sistema de Análisis**: Aplicamos criterio restrictivo - *ningún paquete 8.x* salvo justificación explícita.

1. **Microsoft.Data.SqlClient 5.2.3** → **2.1.7**
   - Serie 5.x requiere netstandard2.1+, incompatible con net472
   - Revertir a última versión estable 2.x

2. **System.Text.Json 8.0.5** → **REMOVER**
   - Serie 8.x no soporta net472
   - Reemplazar por Newtonsoft.Json 13.0.3

3. **Microsoft.Extensions.Logging.Abstractions 7.0.1** → **3.1.29**
   - Alinear con stack EF Core 3.1 para evitar dependency hell

### Alternativas Evaluadas

| Paquete Problemático | Opción A (Recomendada) | Opción B | Opción C |
|---------------------|------------------------|----------|----------|
| System.Text.Json 8.x | Newtonsoft.Json 13.0.3 | System.Text.Json 4.7.2 | Migrar a .NET 6+ |
| Azure.* 1.x | **REMOVER** (no usado) | Mantener + binding redirects | Migrar arquitectura |
| Microsoft.Data.SqlClient 5.x | **Downgrade 2.1.7** | System.Data.SqlClient | Migrar a .NET 6+ |

## Riesgos Identificados

### 🔴 **Alto Riesgo**
- **FileNotFoundException** en runtime por conflictos Microsoft.Bcl.AsyncInterfaces
- **Dependencia circular** potencial si se crea proyecto Controller sin diseño
- **Binding redirect** insuficiente para manejar gaps de versión 3.1→8.0

### 🟡 **Medio Riesgo** 
- **AutoMapper 7.0.1** obsoleto, posibles vulnerabilidades de seguridad
- **Newtonsoft.Json 10.0.1** desactualizado (actual: 13.0.3)

### 🟢 **Bajo Riesgo**
- System.ValueTuple 4.5.0 (estable para net472)
- Configuración de binding redirects correcta

## Estrategia de Migración

### Fase 1: Limpieza (Ejecutar en orden)
```powershell
# 1. Remover paquetes Azure no utilizados
Uninstall-Package Azure.Core -ProjectName DAL
Uninstall-Package Azure.Identity -ProjectName DAL

# 2. Estandarizar Microsoft.Bcl.AsyncInterfaces
Update-Package Microsoft.Bcl.AsyncInterfaces -Version 6.0.0 -ProjectName DAL

# 3. Downgrade System.Runtime.CompilerServices.Unsafe
Update-Package System.Runtime.CompilerServices.Unsafe -Version 4.7.1
```

### Fase 2: Correcciones Core
```powershell
# 4. Actualizar EF Core stack
Update-Package Microsoft.EntityFrameworkCore -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.EntityFrameworkCore.SqlServer -Version 3.1.32 -ProjectName DAL

# 5. Downgrade SqlClient
Update-Package Microsoft.Data.SqlClient -Version 2.1.7 -ProjectName DAL

# 6. Reemplazar System.Text.Json
Uninstall-Package System.Text.Json -ProjectName DAL
Install-Package Newtonsoft.Json -Version 13.0.3 -ProjectName DAL
```

### Fase 3: Optimización
```powershell
# 7. Actualizar AutoMapper
Update-Package AutoMapper -Version 10.1.1 -ProjectName BLL
Update-Package AutoMapper.Extensions.Microsoft.DependencyInjection -Version 8.1.1 -ProjectName BLL

# 8. Alinear Extensions
Update-Package Microsoft.Extensions.Logging.Abstractions -Version 3.1.29 -ProjectName DAL
Update-Package Microsoft.Extensions.Primitives -Version 3.1.29 -ProjectName DAL
```

## Proyecto Controller Faltante

**Problema**: `Program.cs` llama a `AddControllerDependencies()` pero no existe el proyecto.

**Soluciones**:

1. **Opción A (Recomendada)**: Crear proyecto Controller
```xml
<ProjectReference Include="..\Controller\Controller.csproj">
  <Project>{GUID}</Project>
  <Name>Controller</Name>
</ProjectReference>
```

2. **Opción B**: Refactorizar Program.cs para remover dependencia
```csharp
// Remover línea:
// .AddControllerDependencies()
```

## Criterios de Aceptación Post-Aplicación

### ✅ **Compilación Exitosa**
```bash
msbuild SistemaPresupuestario.sln /p:Configuration=Release
# Expected: 0 Errors, 0 Warnings related to assembly loading
```

### ✅ **Testing de Runtime**
- Verificar que la aplicación WinForms inicia sin FileNotFoundException
- Confirmar operaciones de base de datos con EF Core funcionan
- Validar que AutoMapper mappings continúan operando

### ✅ **Binding Redirects Actualizados**
Los app.config deberán reflejar las nuevas versiones. Configurar AutoGenerateBindingRedirects:
```xml
<PropertyGroup>
  <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
  <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
</PropertyGroup>
```

---

**Tiempo Estimado de Aplicación**: 2-3 horas  
**Compatibilidad de Rollback**: Completo (mediante git reset)  
**Riesgo de Regresión**: Bajo (cambios incrementales y conservadores)  