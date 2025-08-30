# NuGet CLI Commands for Sistema Presupuestario
# Execute in order from solution root directory

echo "=== PHASE 1: CLEANUP ==="

# Remove unused Azure packages
nuget.exe uninstall Azure.Core -ProjectName DAL
nuget.exe uninstall Azure.Identity -ProjectName DAL  
nuget.exe uninstall System.ClientModel -ProjectName DAL

# Remove System.Text.Json and dependencies
nuget.exe uninstall System.Text.Json -ProjectName DAL
nuget.exe uninstall System.Text.Encodings.Web -ProjectName DAL
nuget.exe uninstall System.Memory.Data -ProjectName DAL
nuget.exe uninstall System.Security.Cryptography.Pkcs -ProjectName DAL

echo "Phase 1 completed - Unused packages removed"

echo "=== PHASE 2: VERSION STANDARDIZATION ==="

# Standardize Microsoft.Bcl.AsyncInterfaces to 6.0.0
nuget.exe update Microsoft.Bcl.AsyncInterfaces -Version 6.0.0
nuget.exe update System.Runtime.CompilerServices.Unsafe -Version 4.7.1

echo "Phase 2 completed - Versions standardized"

echo "=== PHASE 3: ENTITY FRAMEWORK ==="

# Update EF Core stack to latest 3.1
nuget.exe update Microsoft.EntityFrameworkCore -Version 3.1.32
nuget.exe update Microsoft.EntityFrameworkCore.SqlServer -Version 3.1.32
nuget.exe update Microsoft.Extensions.DependencyInjection -Version 3.1.32
nuget.exe update Microsoft.Extensions.Logging.Abstractions -Version 3.1.32

echo "Phase 3 completed - EF Core updated"

echo "=== PHASE 4: SQL CLIENT ==="

# Downgrade Microsoft.Data.SqlClient
nuget.exe update Microsoft.Data.SqlClient -Version 2.1.7
nuget.exe update System.Diagnostics.DiagnosticSource -Version 4.7.1

echo "Phase 4 completed - SQL Client downgraded"

echo "=== PHASE 5: FINAL UPDATES ==="

# Update AutoMapper
nuget.exe update AutoMapper -Version 10.1.1
nuget.exe update Newtonsoft.Json -Version 13.0.3

echo "Phase 5 completed - Final packages updated"

echo "=== BUILD COMMANDS ==="
echo "1. Remove bin and obj directories:"
echo "   rmdir /s /q */bin */obj"
echo ""
echo "2. Restore packages:"
echo "   nuget.exe restore SistemaPresupuestario.sln"
echo ""  
echo "3. Build solution:"
echo "   msbuild SistemaPresupuestario.sln /p:Configuration=Release"
echo ""
echo "ALL COMMANDS COMPLETED - Check App.config binding redirects"