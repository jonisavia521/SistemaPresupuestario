using System;
using Services.DomainModel.Security.Composite;

namespace Examples
{
    /// <summary>
    /// EJEMPLOS PRÁCTICOS DEL PATRÓN COMPOSITE
    /// 
    /// Esta clase demuestra cómo usar la implementación del patrón Composite
    /// del sistema de permisos con ejemplos paso a paso.
    /// </summary>
    public class CompositePatternExamples
    {
        /// <summary>
        /// EJEMPLO 1: Crear un usuario con permisos básicos
        /// 
        /// Demuestra:
        /// - Creación de patentes individuales
        /// - Asignación directa de permisos al usuario
        /// - Obtención de lista de permisos
        /// </summary>
        public static void EjemploBasico()
        {
            Console.WriteLine("=== EJEMPLO 1: PERMISOS BÁSICOS ===\n");

            // 1. CREAR PATENTES INDIVIDUALES
            var patenteLogin = new Patente
            {
                IdComponent = Guid.NewGuid(),
                FormName = "frmLogin",
                MenuItemName = "mnuLogin"
            };

            var patenteReportes = new Patente
            {
                IdComponent = Guid.NewGuid(),
                FormName = "frmReportes", 
                MenuItemName = "mnuGenerarReporte"
            };

            // 2. CREAR USUARIO
            var empleado = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = "Carlos Empleado",
                User = "cempleado",
                Password = "123456"
            };

            // 3. ASIGNAR PERMISOS DIRECTOS
            empleado.Permisos.Add(patenteLogin);
            empleado.Permisos.Add(patenteReportes);

            // 4. OBTENER Y MOSTRAR PERMISOS
            var permisos = empleado.GetPatentesAll();
            
            Console.WriteLine($"Usuario: {empleado.Nombre}");
            Console.WriteLine($"Permisos ({permisos.Count}):");
            foreach (var permiso in permisos)
            {
                Console.WriteLine($"  - {permiso.FormName}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// EJEMPLO 2: Usuario con familia de permisos
        /// 
        /// Demuestra:
        /// - Creación de familias de permisos
        /// - Agrupación lógica de patentes
        /// - Asignación de familias completas a usuarios
        /// </summary>
        public static void EjemploConFamilias()
        {
            Console.WriteLine("=== EJEMPLO 2: FAMILIAS DE PERMISOS ===\n");

            // 1. CREAR PATENTES RELACIONADAS CON VENTAS
            var patenteFacturacion = new Patente
            {
                IdComponent = Guid.NewGuid(),
                FormName = "frmFacturacion",
                MenuItemName = "mnuCrearFactura"
            };

            var patenteCobranza = new Patente
            {
                IdComponent = Guid.NewGuid(),
                FormName = "frmCobranza",
                MenuItemName = "mnuRegistrarPago"
            };

            var patenteClientes = new Patente
            {
                IdComponent = Guid.NewGuid(),
                FormName = "frmClientes",
                MenuItemName = "mnuGestionClientes"
            };

            // 2. CREAR FAMILIA DE VENTAS
            var familiaVentas = new Familia
            {
                IdComponent = Guid.NewGuid(),
                Nombre = "Módulo de Ventas"
            };

            // 3. AGREGAR PATENTES A LA FAMILIA
            familiaVentas.Add(patenteFacturacion);
            familiaVentas.Add(patenteCobranza);
            familiaVentas.Add(patenteClientes);

            // 4. CREAR USUARIO VENDEDOR
            var vendedor = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = "Ana Vendedora",
                User = "avendedora",
                Password = "ventas123"
            };

            // 5. ASIGNAR FAMILIA COMPLETA + PERMISO INDIVIDUAL
            var patenteLogin = new Patente
            {
                IdComponent = Guid.NewGuid(),
                FormName = "frmLogin",
                MenuItemName = "mnuLogin"
            };

            vendedor.Permisos.Add(patenteLogin);      // Permiso individual
            vendedor.Permisos.Add(familiaVentas);     // Familia completa

            // 6. MOSTRAR ESTRUCTURA Y PERMISOS FINALES
            Console.WriteLine($"Usuario: {vendedor.Nombre}");
            Console.WriteLine("\nEstructura de permisos:");
            Console.WriteLine("├── Patente: Login (individual)");
            Console.WriteLine("└── Familia: Módulo de Ventas");
            Console.WriteLine("    ├── Patente: Facturación");
            Console.WriteLine("    ├── Patente: Cobranza");
            Console.WriteLine("    └── Patente: Clientes");

            // 7. PERMISOS EFECTIVOS (aplanados)
            var permisosEfectivos = vendedor.GetPatentesAll();
            Console.WriteLine($"\nPermisos efectivos ({permisosEfectivos.Count}):");
            foreach (var permiso in permisosEfectivos)
            {
                Console.WriteLine($"  - {permiso.FormName}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// EJEMPLO 3: Jerarquía compleja con múltiples niveles
        /// 
        /// Demuestra:
        /// - Familias que contienen otras familias (jerarquía)
        /// - Recorrido recursivo automático
        /// - Casos de uso realistas para administradores
        /// </summary>
        public static void EjemploJerarquicoComplejo()
        {
            Console.WriteLine("=== EJEMPLO 3: JERARQUÍA COMPLEJA ===\n");

            // === NIVEL 1: PATENTES BÁSICAS ===
            var patenteLogin = new Patente { FormName = "frmLogin", IdComponent = Guid.NewGuid() };

            // === NIVEL 2: PATENTES DE ADMINISTRACIÓN ===
            var patenteUsuarios = new Patente { FormName = "frmUsuarios", IdComponent = Guid.NewGuid() };
            var patenteRoles = new Patente { FormName = "frmRoles", IdComponent = Guid.NewGuid() };

            // === NIVEL 3: PATENTES DE CONFIGURACIÓN ===
            var patenteBackup = new Patente { FormName = "frmBackup", IdComponent = Guid.NewGuid() };
            var patenteLogs = new Patente { FormName = "frmLogs", IdComponent = Guid.NewGuid() };
            var patenteParametros = new Patente { FormName = "frmParametros", IdComponent = Guid.NewGuid() };

            // === CREAR FAMILIA DE CONFIGURACIÓN (NIVEL 3) ===
            var familiaConfiguracion = new Familia
            {
                IdComponent = Guid.NewGuid(),
                Nombre = "Configuración del Sistema"
            };
            familiaConfiguracion.Add(patenteBackup);
            familiaConfiguracion.Add(patenteLogs);
            familiaConfiguracion.Add(patenteParametros);

            // === CREAR FAMILIA DE ADMINISTRACIÓN (NIVEL 2) ===
            var familiaAdministracion = new Familia
            {
                IdComponent = Guid.NewGuid(),
                Nombre = "Administración"
            };
            familiaAdministracion.Add(patenteUsuarios);
            familiaAdministracion.Add(patenteRoles);
            familiaAdministracion.Add(familiaConfiguracion); // ¡FAMILIA DENTRO DE FAMILIA!

            // === PATENTES DE REPORTES ===
            var patenteReporteVentas = new Patente { FormName = "frmReporteVentas", IdComponent = Guid.NewGuid() };
            var patenteReporteUsuarios = new Patente { FormName = "frmReporteUsuarios", IdComponent = Guid.NewGuid() };

            // === CREAR FAMILIA DE REPORTES ===
            var familiaReportes = new Familia
            {
                IdComponent = Guid.NewGuid(),
                Nombre = "Reportes"
            };
            familiaReportes.Add(patenteReporteVentas);
            familiaReportes.Add(patenteReporteUsuarios);

            // === CREAR USUARIO ADMINISTRADOR ===
            var administrador = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = "Luis Administrador",
                User = "ladmin",
                Password = "admin123"
            };

            // === ASIGNAR PERMISOS MIXTOS ===
            administrador.Permisos.Add(patenteLogin);              // Individual
            administrador.Permisos.Add(familiaAdministracion);     // Jerarquía compleja
            administrador.Permisos.Add(familiaReportes);           // Otra familia

            // === MOSTRAR ESTRUCTURA COMPLETA ===
            Console.WriteLine($"Usuario: {administrador.Nombre}");
            Console.WriteLine("\nEstructura jerárquica de permisos:");
            Console.WriteLine("├── 🗝️  Patente: Login (individual)");
            Console.WriteLine("├── 🎗️  Familia: Administración");
            Console.WriteLine("│   ├── 🗝️  Patente: Usuarios");
            Console.WriteLine("│   ├── 🗝️  Patente: Roles");
            Console.WriteLine("│   └── 🎗️  Familia: Configuración del Sistema");
            Console.WriteLine("│       ├── 🗝️  Patente: Backup");
            Console.WriteLine("│       ├── 🗝️  Patente: Logs");
            Console.WriteLine("│       └── 🗝️  Patente: Parámetros");
            Console.WriteLine("└── 🎗️  Familia: Reportes");
            Console.WriteLine("    ├── 🗝️  Patente: Reporte Ventas");
            Console.WriteLine("    └── 🗝️  Patente: Reporte Usuarios");

            // === OBTENER PERMISOS EFECTIVOS ===
            Console.WriteLine("\n¡EL PATRÓN COMPOSITE EN ACCIÓN!");
            Console.WriteLine("Recorriendo automáticamente toda la jerarquía...\n");

            var permisosEfectivos = administrador.GetPatentesAll();
            
            Console.WriteLine($"Permisos efectivos finales ({permisosEfectivos.Count}):");
            foreach (var permiso in permisosEfectivos)
            {
                Console.WriteLine($"  ✓ {permiso.FormName}");
            }

            Console.WriteLine("\n¡El administrador tiene acceso a TODOS los formularios");
            Console.WriteLine("sin importar en qué nivel de la jerarquía estén!");
            Console.WriteLine();
        }

        /// <summary>
        /// EJEMPLO 4: Demostrar las restricciones del patrón
        /// 
        /// Demuestra:
        /// - Qué pasa cuando intentas agregar hijos a una patente
        /// - Manejo de excepciones esperadas
        /// - Diferencias entre Leaf y Composite
        /// </summary>
        public static void EjemploRestricciones()
        {
            Console.WriteLine("=== EJEMPLO 4: RESTRICCIONES DEL PATRÓN ===\n");

            var patente = new Patente 
            { 
                FormName = "frmEjemplo",
                IdComponent = Guid.NewGuid()
            };

            var familia = new Familia 
            { 
                Nombre = "Ejemplo Familia",
                IdComponent = Guid.NewGuid() 
            };

            Console.WriteLine("Comparando comportamiento entre Patente (Leaf) y Familia (Composite):\n");

            // === OPERACIONES EN FAMILIA (COMPOSITE) ===
            Console.WriteLine("✅ FAMILIA (Composite) - Operaciones permitidas:");
            
            try 
            {
                familia.Add(patente);
                Console.WriteLine($"   familia.Add(patente) → OK! Ahora tiene {familia.ChildrenCount()} hijo(s)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   familia.Add(patente) → ERROR: {ex.Message}");
            }

            try 
            {
                var count = familia.ChildrenCount();
                Console.WriteLine($"   familia.ChildrenCount() → OK! Retorna: {count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   familia.ChildrenCount() → ERROR: {ex.Message}");
            }

            // === OPERACIONES EN PATENTE (LEAF) ===
            Console.WriteLine("\n❌ PATENTE (Leaf) - Operaciones restringidas:");

            try 
            {
                patente.Add(new Patente { IdComponent = Guid.NewGuid() });
                Console.WriteLine("   patente.Add(...) → OK! (no debería llegar aquí)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   patente.Add(...) → EXCEPCIÓN ESPERADA: {ex.Message}");
            }

            try 
            {
                var count = patente.ChildrenCount();
                Console.WriteLine($"   patente.ChildrenCount() → OK! Retorna: {count} (siempre 0)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   patente.ChildrenCount() → ERROR: {ex.Message}");
            }

            try 
            {
                patente.Remove(new Patente { IdComponent = Guid.NewGuid() });
                Console.WriteLine("   patente.Remove(...) → OK! (no debería llegar aquí)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   patente.Remove(...) → EXCEPCIÓN ESPERADA: {ex.Message}");
            }

            Console.WriteLine("\n💡 CONCLUSIÓN:");
            Console.WriteLine("   - Las FAMILIAS aceptan operaciones de contenedor (Add, Remove, etc.)");
            Console.WriteLine("   - Las PATENTES rechazan operaciones de contenedor (son elementos terminales)");
            Console.WriteLine("   - Esto mantiene la integridad del patrón Composite");
            Console.WriteLine();
        }

        /// <summary>
        /// MÉTODO PRINCIPAL PARA EJECUTAR TODOS LOS EJEMPLOS
        /// </summary>
        public static void EjecutarTodosLosEjemplos()
        {
            Console.WriteLine("🚀 DEMOSTRACIONES DEL PATRÓN COMPOSITE");
            Console.WriteLine("=====================================\n");

            EjemploBasico();
            EjemploConFamilias();
            EjemploJerarquicoComplejo();
            EjemploRestricciones();

            Console.WriteLine("✨ ¡Todos los ejemplos completados!");
            Console.WriteLine("El patrón Composite permite manejar estructuras jerárquicas");
            Console.WriteLine("de permisos de forma simple, flexible y extensible.");
        }
    }
}