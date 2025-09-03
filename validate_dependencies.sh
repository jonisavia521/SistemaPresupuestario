#!/bin/bash

echo "=== Sistema Presupuestario Dependency Validation ==="
echo ""

# Function to check if a binding redirect exists and is correct
check_binding_redirect() {
    local file=$1
    local assembly=$2
    local expected_version=$3
    
    echo "Checking $assembly in $file..."
    
    if grep -q "assemblyIdentity name=\"$assembly\"" "$file"; then
        if grep -A 2 "assemblyIdentity name=\"$assembly\"" "$file" | grep -q "newVersion=\"$expected_version\""; then
            echo "  ✅ $assembly -> $expected_version"
        else
            echo "  ❌ $assembly binding redirect incorrect"
            return 1
        fi
    else
        echo "  ⚠️  $assembly binding redirect not found"
        return 1
    fi
}

# Function to check package version in packages.config
check_package_version() {
    local file=$1
    local package=$2
    local expected_version=$3
    
    echo "Checking $package in $file..."
    
    if grep -q "id=\"$package\"" "$file"; then
        if grep "id=\"$package\"" "$file" | grep -q "version=\"$expected_version\""; then
            echo "  ✅ $package version $expected_version"
        else
            echo "  ❌ $package version mismatch"
            actual_version=$(grep "id=\"$package\"" "$file" | sed -n 's/.*version="\([^"]*\)".*/\1/p')
            echo "     Expected: $expected_version, Found: $actual_version"
            return 1
        fi
    else
        echo "  ⚠️  $package not found"
        return 1
    fi
}

cd "$(dirname "$0")"

echo "### Phase 1: Checking package versions ###"
echo ""

# Check DAL packages
echo "-- DAL Project --"
check_package_version "DAL/packages.config" "Microsoft.Extensions.Caching.Memory" "3.1.29"
check_package_version "DAL/packages.config" "Microsoft.Extensions.Primitives" "3.1.29"  
check_package_version "DAL/packages.config" "Microsoft.Extensions.Logging.Abstractions" "3.1.29"
check_package_version "DAL/packages.config" "Microsoft.Data.SqlClient" "2.1.7"
check_package_version "DAL/packages.config" "Microsoft.Data.SqlClient.SNI" "2.1.1"
check_package_version "DAL/packages.config" "Microsoft.Bcl.AsyncInterfaces" "6.0.0"

echo ""

# Check Services packages
echo "-- Services Project --" 
check_package_version "Services/packages.config" "Microsoft.Data.SqlClient.SNI" "2.1.1"
check_package_version "Services/packages.config" "Microsoft.Bcl.AsyncInterfaces" "6.0.0"

echo ""

echo "### Phase 2: Checking binding redirects ###"
echo ""

# Check main App.config
echo "-- Main App.config --"
check_binding_redirect "SistemaPresupuestario/App.config" "Microsoft.Extensions.Caching.Memory" "3.1.29.0"
check_binding_redirect "SistemaPresupuestario/App.config" "Microsoft.Extensions.Primitives" "3.1.29.0"
check_binding_redirect "SistemaPresupuestario/App.config" "Microsoft.Extensions.Logging.Abstractions" "3.1.29.0"
check_binding_redirect "SistemaPresupuestario/App.config" "Microsoft.Data.SqlClient" "2.1.7.0"
check_binding_redirect "SistemaPresupuestario/App.config" "Microsoft.Bcl.AsyncInterfaces" "6.0.0.0"

echo ""

# Check DAL app.config  
echo "-- DAL app.config --"
check_binding_redirect "DAL/app.config" "Microsoft.Extensions.Caching.Memory" "3.1.29.0"
check_binding_redirect "DAL/app.config" "Microsoft.Extensions.Primitives" "3.1.29.0"
check_binding_redirect "DAL/app.config" "Microsoft.Extensions.Logging.Abstractions" "3.1.29.0"
check_binding_redirect "DAL/app.config" "Microsoft.Data.SqlClient" "2.1.7.0"

echo ""

echo "### Phase 3: Checking platform configuration ###"
echo ""

# Check platform target
echo "-- Platform Configuration --"
if grep -q "<PlatformTarget>x64</PlatformTarget>" "SistemaPresupuestario/SistemaPresupuestario.csproj"; then
    echo "  ✅ Platform target set to x64"
else
    echo "  ❌ Platform target not set to x64"
fi

if grep -q "<Prefer32Bit>false</Prefer32Bit>" "SistemaPresupuestario/SistemaPresupuestario.csproj"; then
    echo "  ✅ Prefer32Bit disabled"
else
    echo "  ❌ Prefer32Bit not disabled"
fi

echo ""

echo "### Phase 4: Checking auto-binding redirects ###"
echo ""

# Check auto-binding redirect settings
for project in "DAL/DAL.csproj" "BLL/BLL.csproj" "Services/Services.csproj"; do
    echo "-- $project --"
    if grep -q "<AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>" "$project"; then
        echo "  ✅ AutoGenerateBindingRedirects enabled"
    else
        echo "  ❌ AutoGenerateBindingRedirects not enabled"
    fi
done

echo ""
echo "=== Validation Complete ==="
echo ""
echo "If all checks show ✅, the dependency issues should be resolved."
echo "Any ❌ items need to be addressed before testing."