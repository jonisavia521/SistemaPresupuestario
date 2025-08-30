# Comandos Package Manager Console para Sistema Presupuestario
# Ejecutar en Visual Studio Package Manager Console en el orden especificado

# FASE 1: LIMPIEZA - Remover paquetes no utilizados
Write-Host "=== FASE 1: LIMPIEZA ===" -ForegroundColor Green

# Remover Azure packages no utilizados del proyecto DAL
Uninstall-Package Azure.Core -ProjectName DAL -Force
Uninstall-Package Azure.Identity -ProjectName DAL -Force
Uninstall-Package System.ClientModel -ProjectName DAL -Force

# Remover System.Text.Json y dependencias (reemplazar por Newtonsoft.Json)
Uninstall-Package System.Text.Json -ProjectName DAL -Force
Uninstall-Package System.Text.Encodings.Web -ProjectName DAL -Force
Uninstall-Package System.Memory.Data -ProjectName DAL -Force
Uninstall-Package System.Security.Cryptography.Pkcs -ProjectName DAL -Force

Write-Host "Fase 1 completada - Paquetes no utilizados removidos" -ForegroundColor Yellow

# FASE 2: ESTANDARIZACIÓN DE VERSIONES
Write-Host "=== FASE 2: ESTANDARIZACIÓN ===" -ForegroundColor Green

# Estandarizar Microsoft.Bcl.AsyncInterfaces a 6.0.0 en todos los proyectos
Update-Package Microsoft.Bcl.AsyncInterfaces -Version 6.0.0 -ProjectName DAL
Update-Package Microsoft.Bcl.AsyncInterfaces -Version 6.0.0 -ProjectName BLL
Update-Package Microsoft.Bcl.AsyncInterfaces -Version 6.0.0 -ProjectName Services
Update-Package Microsoft.Bcl.AsyncInterfaces -Version 6.0.0 -ProjectName SistemaPresupuestario

# Downgrade System.Runtime.CompilerServices.Unsafe a versión compatible con net472
Update-Package System.Runtime.CompilerServices.Unsafe -Version 4.7.1 -ProjectName DAL
Update-Package System.Runtime.CompilerServices.Unsafe -Version 4.7.1 -ProjectName BLL
Update-Package System.Runtime.CompilerServices.Unsafe -Version 4.7.1 -ProjectName Services
Update-Package System.Runtime.CompilerServices.Unsafe -Version 4.7.1 -ProjectName SistemaPresupuestario

Write-Host "Fase 2 completada - Versiones estandarizadas" -ForegroundColor Yellow

# FASE 3: ACTUALIZACIONES DE ENTITY FRAMEWORK
Write-Host "=== FASE 3: ENTITY FRAMEWORK ===" -ForegroundColor Green

# Actualizar EF Core stack a la última versión 3.1
Update-Package Microsoft.EntityFrameworkCore -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.EntityFrameworkCore.Abstractions -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.EntityFrameworkCore.Relational -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.EntityFrameworkCore.SqlServer -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.EntityFrameworkCore.Design -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.EntityFrameworkCore.Tools -Version 3.1.32 -ProjectName DAL

# Actualizar Microsoft.Extensions stack para alinear con EF Core 3.1
Update-Package Microsoft.Extensions.DependencyInjection -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.Extensions.DependencyInjection.Abstractions -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.Extensions.Logging -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.Extensions.Logging.Abstractions -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.Extensions.Caching.Abstractions -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.Extensions.Caching.Memory -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.Extensions.Configuration -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.Extensions.Configuration.Abstractions -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.Extensions.Configuration.Binder -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.Extensions.Options -Version 3.1.32 -ProjectName DAL
Update-Package Microsoft.Extensions.Primitives -Version 3.1.32 -ProjectName DAL

Write-Host "Fase 3 completada - EF Core stack actualizado" -ForegroundColor Yellow

# FASE 4: SQL CLIENT Y DEPENDENCIAS
Write-Host "=== FASE 4: SQL CLIENT ===" -ForegroundColor Green

# Downgrade Microsoft.Data.SqlClient a versión compatible con net472
Update-Package Microsoft.Data.SqlClient -Version 2.1.7 -ProjectName DAL
Update-Package Microsoft.Data.SqlClient.SNI -Version 2.1.1 -ProjectName DAL
Update-Package Microsoft.Data.SqlClient.SNI -Version 2.1.1 -ProjectName Services

# Downgrade System.Diagnostics.DiagnosticSource
Update-Package System.Diagnostics.DiagnosticSource -Version 4.7.1 -ProjectName DAL

Write-Host "Fase 4 completada - SQL Client downgraded" -ForegroundColor Yellow

# FASE 5: AUTOMAPPER Y JSON
Write-Host "=== FASE 5: AUTOMAPPER Y JSON ===" -ForegroundColor Green

# Actualizar AutoMapper a la última versión compatible con net472
Update-Package AutoMapper -Version 10.1.1 -ProjectName BLL
Update-Package AutoMapper.Extensions.Microsoft.DependencyInjection -Version 8.1.1 -ProjectName BLL

# Actualizar Newtonsoft.Json
Update-Package Newtonsoft.Json -Version 13.0.3 -ProjectName DAL

Write-Host "Fase 5 completada - AutoMapper y JSON actualizados" -ForegroundColor Yellow

# VERIFICACIÓN FINAL
Write-Host "=== VERIFICACIÓN FINAL ===" -ForegroundColor Green

Write-Host "Ejecutar para limpiar y rebuildar:" -ForegroundColor Cyan
Write-Host "1. Clean Solution" -ForegroundColor White
Write-Host "2. Delete bin/ y obj/ folders manualmente" -ForegroundColor White
Write-Host "3. nuget restore SistemaPresupuestario.sln" -ForegroundColor White
Write-Host "4. Rebuild Solution" -ForegroundColor White

Write-Host ""
Write-Host "COMANDOS COMPLETADOS - Revisar binding redirects en App.config" -ForegroundColor Green
Write-Host "Si hay errores de compilación, verificar que AutoGenerateBindingRedirects=true" -ForegroundColor Yellow