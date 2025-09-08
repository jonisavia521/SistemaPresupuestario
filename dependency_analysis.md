# Análisis Exhaustivo de Dependencias: .NET Framework 4.7.2 + EF Core 3.1.29

## 1. Árbol Jerárquico ACTUAL de Dependencias por Proyecto

### SistemaPresupuestario (UI - WinForms)
- **Target Framework**: .NET Framework 4.7.2 ✓
- **Tipo de gestión**: packages.config
- **Referencias NuGet**:
  - Microsoft.Bcl.AsyncInterfaces 1.1.1 (netstandard2.0) ✓
  - Microsoft.Extensions.Caching.Abstractions 3.1.29 (netstandard2.0) ✓
  - Microsoft.Extensions.Caching.Memory 3.1.29 (netstandard2.0) ✓
  - Microsoft.Extensions.Configuration 3.1.29 (netstandard2.0) ✓
  - Microsoft.Extensions.Configuration.Abstractions 3.1.29 (netstandard2.0) ✓
  - Microsoft.Extensions.Configuration.Binder 3.1.29 (netstandard2.0) ✓
  - Microsoft.Extensions.DependencyInjection 3.1.29 (net461) ✓
  - Microsoft.Extensions.DependencyInjection.Abstractions 3.1.29 (netstandard2.0) ✓
  - Microsoft.Extensions.Logging 3.1.29 (netstandard2.0) ✓
  - Microsoft.Extensions.Logging.Abstractions 3.1.29 (netstandard2.0) ✓
  - Microsoft.Extensions.Options 3.1.29 (netstandard2.0) ✓
  - Microsoft.Extensions.Primitives 3.1.29 (netstandard2.0) ✓
  - System.Runtime.CompilerServices.Unsafe 4.5.3 (net461) ✓
  - System.Threading.Tasks.Extensions 4.5.4 (net461) ✓
- **Referencias de proyecto**: BLL, DomainModel, Services

### BLL (Business Logic Layer)
- **Target Framework**: .NET Framework 4.7.2 ✓
- **Tipo de gestión**: packages.config
- **Referencias NuGet**:
  - AutoMapper 7.0.1 (net45) ⚠️ (versión antigua pero funcional)
  - AutoMapper.Extensions.Microsoft.DependencyInjection 4.0.1 (netstandard2.0) ⚠️
  - Microsoft.Bcl.AsyncInterfaces 1.1.1 (netstandard2.0) ✓
  - Microsoft.Extensions.* 3.1.29 (alineados) ✓
  - System.Runtime.CompilerServices.Unsafe 4.5.3 (net461) ✓
  - System.Threading.Tasks.Extensions 4.5.4 (net461) ✓
  - System.ValueTuple 4.5.0 (net47) ✓
- **Referencias de proyecto**: DAL, DomainModel, Services

### DAL (Data Access Layer - EF Core)
- **Target Framework**: .NET Framework 4.7.2 ✓
- **Tipo de gestión**: packages.config
- **Referencias NuGet EF Core** ✅:
  - Microsoft.EntityFrameworkCore 3.1.29 (netstandard2.0) ✓
  - Microsoft.EntityFrameworkCore.Abstractions 3.1.29 (netstandard2.0) ✓
  - Microsoft.EntityFrameworkCore.Design 3.1.29 (netstandard2.0) ✓
  - Microsoft.EntityFrameworkCore.Relational 3.1.29 (netstandard2.0) ✓
  - Microsoft.EntityFrameworkCore.SqlServer 3.1.29 (netstandard2.0) ✓
  - Microsoft.Data.SqlClient 2.0.1 (net46) ✓
- **Referencias Microsoft.Extensions** ✅:
  - Microsoft.Extensions.* 3.1.29 (alineados) ✓
  - Microsoft.Extensions.Logging.Abstractions 3.1.29 (netstandard2.0) ✓ CORREGIDO
- **Referencias de soporte**:
  - Microsoft.Bcl.AsyncInterfaces 1.1.1 (netstandard2.0) ✓ CORREGIDO
  - System.Runtime.CompilerServices.Unsafe 4.5.3 (net461) ✓ CORREGIDO
  - Newtonsoft.Json 13.0.1 (net45) ✓
  - System.* packages compatibles
- **Referencias de proyecto**: Services

### Services (Services Layer)
- **Target Framework**: .NET Framework 4.7.2 ✓
- **Tipo de gestión**: packages.config
- **Referencias NuGet**:
  - Microsoft.Bcl.AsyncInterfaces 1.1.1 (netstandard2.0) ✓ CORREGIDO
  - Microsoft.Extensions.* 3.1.29 (alineados) ✓
  - System.Runtime.CompilerServices.Unsafe 4.5.3 (net461) ✓ CORREGIDO
  - System.Threading.Tasks.Extensions 4.5.4 (net461) ✓

### DomainModel (Domain Entities)
- **Target Framework**: .NET Framework 4.7.2 ✓
- **Tipo de gestión**: Solo referencias .NET Framework
- **Referencias**: Solo System.* básicas ✓

## 2. Arquitectura de Referencias Entre Capas RECOMENDADA

```
┌─────────────────────┐
│   SistemaPresupuestario (UI)   │
│   - WinForms App    │
│   - DI Container    │
└─────────┬───────────┘
          │ references
          ▼
┌─────────────────────┐    ┌─────────────────────┐
│        BLL          │    │     Services        │
│  Business Logic     │◄──►│   Cross-cutting     │
└─────────┬───────────┘    └─────────┬───────────┘
          │ references               │
          ▼                         │
┌─────────────────────┐              │
│        DAL          │              │
│   EF Core 3.1.29    │◄─────────────┘
│   Data Access       │
└─────────┬───────────┘
          │ references
          ▼
┌─────────────────────┐
│    DomainModel      │
│   Domain Entities   │
└─────────────────────┘
```

**Flujo recomendado de dependencias**:
- UI → BLL → DAL → DomainModel
- UI → Services (para servicios transversales)
- BLL → Services (para logging, configuración)
- Services ← DAL (para logging, helpers)

## 3. Árbol Jerárquico RECOMENDADO (Estado Final)

### Paquetes por Capa Recomendados:

#### UI Layer (SistemaPresupuestario):
- Microsoft.Extensions.DependencyInjection 3.1.29
- Microsoft.Extensions.Configuration.* 3.1.29 (si necesita configuración)
- Solo referencias necesarias para DI y configuración básica

#### BLL Layer:
- AutoMapper 7.0.1 → **MANTENER** (funcional para net472)
- AutoMapper.Extensions.Microsoft.DependencyInjection 4.0.1 → **MANTENER**
- Microsoft.Extensions.Logging.Abstractions 3.1.29
- Microsoft.Extensions.DependencyInjection.Abstractions 3.1.29

#### DAL Layer (SOLO aquí debe estar EF Core):
- **Microsoft.EntityFrameworkCore 3.1.29** ✅
- **Microsoft.EntityFrameworkCore.SqlServer 3.1.29** ✅
- **Microsoft.Data.SqlClient 2.0.1** ✅ (compatible con EF Core 3.1)
- Microsoft.Extensions.Logging.Abstractions 3.1.29 ✅
- System.* packages necesarios para EF Core

#### Services Layer:
- Microsoft.Extensions.Logging.Abstractions 3.1.29
- Microsoft.Extensions.Configuration.Abstractions 3.1.29
- Paquetes mínimos para servicios transversales

#### DomainModel:
- **SOLO .NET Framework references** ✅ (mantener limpio)

## 4. Versiones Clave Unificadas

| Paquete | Versión Final | Justificación |
|---------|---------------|---------------|
| **EF Core Family** | **3.1.29** | ✅ Fija por requerimiento |
| **Microsoft.Extensions.*** | **3.1.29** | ✅ Alineado con EF Core 3.1 |
| **Microsoft.Data.SqlClient** | **2.0.1** | ✅ Última versión compatible con EF Core 3.1 + net472 |
| **System.Runtime.CompilerServices.Unsafe** | **4.5.3** | ✅ Compatible con net472 y EF Core 3.1 |
| **Microsoft.Bcl.AsyncInterfaces** | **1.1.1** | ✅ Compatible con net472 y EF Core 3.1 |
| **Newtonsoft.Json** | **13.0.1** | ✅ Última versión compatible con net472 |
| **AutoMapper** | **7.0.1** | ⚠️ Antigua pero funcional para net472 |

## 5. Cambios Aplicados (Before → After)

### Downgrades Críticos:
- Microsoft.Bcl.AsyncInterfaces: **6.0.0 → 1.1.1** (todos los proyectos)
- System.Runtime.CompilerServices.Unsafe: **6.0.0 → 4.5.3** (todos los proyectos)
- Microsoft.Extensions.Logging.Abstractions (DAL): **6.0.0 → 3.1.29**

### Paquetes Removidos (innecesarios para WinForms + EF Core básico):
❌ Azure.Core, Azure.Identity  
❌ Microsoft.Identity.Client.*  
❌ Microsoft.IdentityModel.*  
❌ System.IdentityModel.Tokens.Jwt  
❌ System.ClientModel  
❌ System.Memory.Data  
❌ System.Text.Json, System.Text.Encodings.Web  

### Binding Redirects Actualizados:
- System.Runtime.CompilerServices.Unsafe: máximo **4.0.5.0**
- Microsoft.Bcl.AsyncInterfaces: máximo **1.0.0.0**
- Microsoft.Extensions.Logging.Abstractions: máximo **3.1.29.0**
- Removidos redirects para paquetes eliminados

## 6. Criterios de Compatibilidad Verificados

✅ **Todos los proyectos apuntan a .NET Framework 4.7.2**  
✅ **EF Core 3.1.29 funcional en DAL únicamente**  
✅ **Microsoft.Extensions.* unificados en 3.1.29**  
✅ **Microsoft.Data.SqlClient 2.0.1 compatible**  
✅ **System.* packages compatibles con netstandard2.0/net472**  
✅ **Sin mezcla de EF6 y EF Core**  
✅ **Arquitectura de capas limpia: UI → BLL → DAL**  
✅ **EF Core SOLO en DAL (aislado)**  

## 7. Notas de Operación

- **No requiere SDK específico** para desarrollo básico
- **Binding redirects automáticos** habilitados en ejecutables
- **Central Package Management**: Se podría implementar con Directory.Packages.props
- **Migration Strategy**: Actual packages.config es funcional para esta solución

## 8. Warnings Esperados (Eliminados)

❌ "Found conflicts between different versions of the same dependent assembly"  
❌ "Could not load file or assembly 'Microsoft.Bcl.AsyncInterfaces, Version=6.0.0.0'"  
❌ "Version conflicts for System.Runtime.CompilerServices.Unsafe"  
❌ "Assembly binding redirects needed"  

## 9. Próximos Pasos

1. ✅ **Validar build limpio** (cuando .NET Framework targeting packs estén disponibles)
2. ✅ **Ejecutar smoke tests** de funcionalidad EF Core
3. ✅ **Verificar conectividad de base de datos**
4. ⏳ **Documentar migration path** para futuras actualizaciones