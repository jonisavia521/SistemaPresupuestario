using Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DomainModel.Security.Composite
{
    /// <summary>
    /// PATRÓN COMPOSITE - CLASE CLIENTE (Client)
    /// 
    /// Representa un usuario del sistema que utiliza la estructura Composite
    /// de permisos para gestionar sus autorizaciones de forma jerárquica.
    /// 
    /// ANALOGÍA COLOQUIAL:
    /// Un Usuario es como una PERSONA que tiene acceso a ciertas "áreas" de un edificio:
    /// - Puede tener llaves individuales para oficinas específicas (Patentes)
    /// - Puede tener tarjetas maestras que dan acceso a múltiples áreas (Familias)
    /// - Las tarjetas maestras pueden incluir acceso a otras tarjetas maestras
    /// - Al final, lo que importa es a qué oficinas específicas puede acceder
    /// 
    /// EJEMPLO PRÁCTICO DE PERMISOS DE USUARIO:
    /// 
    /// Usuario: "Juan Pérez" (Administrador)
    /// ├── Familia: "Administración"
    /// │   ├── Patente: "CrearUsuarios" 
    /// │   ├── Patente: "EliminarUsuarios"
    /// │   └── Familia: "Configuración"
    /// │       ├── Patente: "CambiarConfiguracion"
    /// │       └── Patente: "VerLogs"
    /// ├── Familia: "Reportes"
    /// │   ├── Patente: "GenerarReporte"
    /// │   └── Patente: "ExportarReporte"
    /// └── Patente: "Login" (permiso individual)
    /// 
    /// BENEFICIOS DEL PATRÓN EN ESTE CONTEXTO:
    /// 1. Flexibilidad: Un usuario puede tener permisos individuales Y grupos de permisos
    /// 2. Reutilización: Las familias de permisos se pueden asignar a múltiples usuarios
    /// 3. Mantenimiento: Fácil agregar/quitar permisos sin cambiar la estructura
    /// 4. Escalabilidad: Jerarquías de cualquier profundidad
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único del usuario en el sistema
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre completo o descriptivo del usuario
        /// Ejemplo: "Juan Pérez", "María González"
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Nombre de usuario para el login (username)
        /// Ejemplo: "jperez", "mgonzalez"
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Contraseña del usuario (debería almacenarse hasheada)
        /// </summary>
        [Browsable(false)]
        public string Password { get; set; }

        /// <summary>
        /// ESTRUCTURA COMPOSITE DE PERMISOS
        /// 
        /// Esta es la colección que implementa el patrón Composite.
        /// Puede contener tanto elementos Leaf (Patentes) como elementos 
        /// Composite (Familias), tratándolos de manera uniforme.
        /// 
        /// VENTAJAS DE USAR COMPOSITE AQUÍ:
        /// - Simplicidad: El usuario no necesita distinguir entre permisos individuales y grupos
        /// - Flexibilidad: Pueden mezclarse patentes individuales con familias de permisos
        /// - Extensibilidad: Fácil agregar nuevos tipos de permisos en el futuro
        /// 
        /// EJEMPLO DE CONTENIDO:
        /// Permisos[0] = new Patente { FormName = "frmLogin" }
        /// Permisos[1] = new Familia { Nombre = "Administración", Hijos = [...] }
        /// Permisos[2] = new Patente { FormName = "frmPerfil" }
        /// </summary>
        public List<Component> Permisos { get; set; }

        /// <summary>
        /// Constructor que inicializa la lista de permisos vacía
        /// </summary>
        public Usuario()
        {
            Permisos = new List<Component>();
        }

        /// <summary>
        /// Hash de integridad que combina datos del usuario
        /// Usado para detectar modificaciones no autorizadas
        /// </summary>
        [Browsable(false)]
        public string HashDH
        {
            get
            {
                return CryptographyService.HashPassword(Nombre + User + Password);
            }
        }

        /// <summary>
        /// Hash de la contraseña del usuario
        /// Usado para verificación de login segura
        /// </summary>
        [Browsable(false)]
        public string HashPassword
        {
            get
            {
                return CryptographyService.HashPassword(this.Password);
            }
        }

        /// <summary>
        /// MÉTODO PRINCIPAL QUE UTILIZA EL PATRÓN COMPOSITE
        /// 
        /// Extrae todas las patentes únicas del usuario recorriendo recursivamente
        /// toda la estructura jerárquica de permisos.
        /// 
        /// PROPÓSITO:
        /// Obtener la lista "aplanada" de todos los permisos específicos que tiene
        /// el usuario, sin importar si vienen de patentes individuales o están
        /// agrupados dentro de familias de permisos.
        /// 
        /// CASO DE USO TÍPICO:
        /// Para construir el menú de la aplicación, mostrando solo las opciones
        /// a las que el usuario tiene acceso.
        /// 
        /// EJEMPLO DEL ALGORITMO:
        /// 
        /// Entrada (estructura jerárquica):
        /// Usuario.Permisos:
        /// ├── Patente: "Login"
        /// └── Familia: "Administración" 
        ///     ├── Patente: "CrearUsuarios"
        ///     └── Familia: "Configuración"
        ///         └── Patente: "VerLogs"
        /// 
        /// Salida (lista aplanada):
        /// ["Login", "CrearUsuarios", "VerLogs"]
        /// 
        /// BENEFICIO DEL PATRÓN COMPOSITE:
        /// Este método puede procesar cualquier estructura jerárquica sin conocer
        /// de antemano su complejidad o profundidad.
        /// </summary>
        /// <returns>Lista de patentes únicas que el usuario puede acceder</returns>
        public List<Patente> GetPatentesAll()
        {
            List<Patente> patentesDistinct = new List<Patente>();

            // ALGORITMO RECURSIVO QUE APROVECHA EL PATRÓN COMPOSITE
            // Comienza el recorrido desde el nivel raíz de permisos del usuario
            RecorrerComposite(patentesDistinct, Permisos, "-");

            return patentesDistinct;
        }

        /// <summary>
        /// ALGORITMO RECURSIVO DEL PATRÓN COMPOSITE
        /// 
        /// Este método implementa el recorrido típico de una estructura Composite,
        /// procesando de manera uniforme tanto elementos Leaf (Patentes) como
        /// elementos Composite (Familias).
        /// 
        /// FUNCIONAMIENTO DEL ALGORITMO:
        /// 
        /// 1. Para cada componente en la lista:
        ///    a) Si ChildrenCount() == 0 → Es una Patente (Leaf)
        ///       - Procesar como permiso individual
        ///       - Agregarlo a la lista si no existe
        ///    b) Si ChildrenCount() > 0 → Es una Familia (Composite) 
        ///       - Procesar como contenedor
        ///       - Llamar recursivamente con sus hijos
        /// 
        /// VENTAJAS DE ESTE ENFOQUE:
        /// - No necesita conocer los tipos concretos (Patente/Familia)
        /// - Funciona con cualquier profundidad de jerarquía
        /// - Es extensible a nuevos tipos de componentes
        /// - Trata uniformemente elementos simples y compuestos
        /// 
        /// ANALOGÍA COLOQUIAL:
        /// Es como buscar todos los archivos en un sistema de archivos:
        /// - Si encuentras un archivo, lo agregas a la lista
        /// - Si encuentras una carpeta, entras y sigues buscando
        /// - Repites hasta revisar todo
        /// 
        /// EJEMPLO DE EJECUCIÓN:
        /// 
        /// Llamada inicial: RecorrerComposite(patentes, usuario.Permisos, "-")
        /// 
        /// Paso 1: Encuentra Patente "Login"
        ///   → Salida: "- Patente: Login"
        ///   → Agrega a patentes si no existe
        /// 
        /// Paso 2: Encuentra Familia "Administración" 
        ///   → Salida: "- Familia: Administración"
        ///   → Llamada recursiva: RecorrerComposite(patentes, familia.hijos, "--")
        ///   
        ///   Paso 2.1: Encuentra Patente "CrearUsuarios"
        ///     → Salida: "-- Patente: CrearUsuarios" 
        ///     → Agrega a patentes si no existe
        ///   
        ///   Paso 2.2: Encuentra Familia "Configuración"
        ///     → Salida: "-- Familia: Configuración"
        ///     → Llamada recursiva: RecorrerComposite(patentes, familia.hijos, "---")
        ///     
        ///     Paso 2.2.1: Encuentra Patente "VerLogs"
        ///       → Salida: "--- Patente: VerLogs"
        ///       → Agrega a patentes si no existe
        /// </summary>
        /// <param name="patentes">Lista acumulativa donde se guardan las patentes encontradas</param>
        /// <param name="components">Lista de componentes a procesar en este nivel</param>
        /// <param name="tab">String para mostrar indentación en la consola (debugging)</param>
        private static void RecorrerComposite(List<Patente> patentes, List<Component> components, string tab)
        {
            foreach (var item in components)
            {
                // DECISIÓN CLAVE DEL PATRÓN COMPOSITE:
                // Usar ChildrenCount() para determinar el tipo sin casting explícito
                if (item.ChildrenCount() == 0)
                {
                    // PROCESAMIENTO DE ELEMENTO LEAF (Patente)
                    // Estoy ante un elemento de tipo Patente (no tiene hijos)
                    Patente patente1 = item as Patente;
                    Console.WriteLine($"{tab} Patente: {patente1.FormName}");

                    // EVITAR DUPLICADOS:
                    // Solo agregar si no existe una patente con el mismo FormName
                    // Esto es importante porque el mismo permiso podría estar en múltiples familias
                    if (!patentes.Exists(o => o.FormName == patente1.FormName))
                        patentes.Add(patente1);

                    // CÓDIGO COMENTADO: Implementación alternativa más verbosa para evitar duplicados
                    // Se mantiene como referencia de una implementación menos eficiente
                    //bool existe = false;
                    //foreach (var item2 in patentes)
                    //{
                    //    if(item2.FormName == patente1.FormName)
                    //    {
                    //        existe = true;
                    //        break;
                    //    }
                    //}
                    //if(!existe)
                    //    patentes.Add(patente1);
                }
                else
                {
                    // PROCESAMIENTO DE ELEMENTO COMPOSITE (Familia)
                    // Estoy ante un elemento de tipo Familia (tiene hijos)
                    Familia familia = item as Familia;
                    Console.WriteLine($"{tab} Familia: {familia.Nombre}");

                    // RECURSIÓN: Procesar los hijos de esta familia
                    // Aumentar la indentación para mostrar la jerarquía
                    RecorrerComposite(patentes, familia.GetChildrens(), tab + "-");
                }
            }
        }
    }
}
