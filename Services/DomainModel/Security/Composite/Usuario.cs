using Services.DomainModel.Security.Composite;
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
    /// Representa un usuario del sistema que utiliza la estructura Composite
    /// de permisos para gestionar sus autorizaciones de forma jerárquica mediante
    /// el patrón de diseño Composite.
    /// 
    /// Analogía: Un Usuario es como una persona que tiene acceso a ciertas áreas de un edificio.
    /// Puede tener llaves individuales para oficinas específicas (Patentes) o tarjetas maestras 
    /// que dan acceso a múltiples áreas (Familias). Las tarjetas maestras pueden incluir acceso 
    /// a otras tarjetas maestras.
    /// 
    /// Ejemplo de estructura de permisos de usuario:
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
    /// Beneficios del patrón en este contexto:
    /// 1. Flexibilidad: Un usuario puede tener permisos individuales Y grupos de permisos
    /// 2. Reutilización: Las familias de permisos se pueden asignar a múltiples usuarios
    /// 3. Mantenimiento: Fácil agregar/quitar permisos sin cambiar la estructura
    /// 4. Escalabilidad: Jerarquías de cualquier profundidad
    /// </summary>
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string User { get; set; }

        [Browsable(false)]
        public string Password { get; set; }

        /// <summary>
        /// Estructura Composite de permisos del usuario.
        /// Puede contener tanto elementos Leaf (Patentes) como elementos 
        /// Composite (Familias), tratándolos de manera uniforme.
        /// </summary>
        public List<Component> Permisos { get; set; }

        public Usuario()
        {
            Permisos = new List<Component>();
        }

        /// <summary>
        /// Hash de integridad que combina datos del usuario.
        /// Usado para detectar modificaciones no autorizadas.
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
        /// Hash de la contraseña del usuario para verificación de login segura
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
        /// Extrae todas las patentes del usuario recorriendo recursivamente
        /// la estructura jerárquica de permisos utilizando el patrón Composite.
        /// 
        /// Propósito: Obtener la lista "aplanada" de todos los permisos específicos del usuario,
        /// sin importar si vienen de patentes individuales o están agrupados dentro de familias.
        /// 
        /// Uso típico: Para construir el menú de la aplicación, mostrando solo las opciones
        /// a las que el usuario tiene acceso.
        /// </summary>
        /// <returns>Lista de patentes únicas que el usuario puede acceder</returns>
        public List<Patente> GetPatentesAll()
        {
            List<Patente> patentesDistinct = new List<Patente>();
            RecorrerComposite(patentesDistinct, Permisos, "-");
            return patentesDistinct;
        }

        /// <summary>
        /// Algoritmo recursivo que recorre la estructura Composite de permisos.
        /// Procesa de manera uniforme tanto elementos Leaf (Patentes) como
        /// elementos Composite (Familias).
        /// 
        /// Funcionamiento:
        /// 1. Para cada componente en la lista:
        ///    a) Si ChildrenCount() == 0 → Es una Patente (Leaf)
        ///       - Procesar como permiso individual
        ///       - Agregarlo a la lista si no existe
        ///    b) Si ChildrenCount() > 0 → Es una Familia (Composite) 
        ///       - Procesar como contenedor
        ///       - Llamar recursivamente con sus hijos
        /// </summary>
        /// <param name="patentes">Lista acumulativa donde se guardan las patentes encontradas</param>
        /// <param name="components">Lista de componentes a procesar en este nivel</param>
        /// <param name="tab">String para indentación en debugging</param>
        private static void RecorrerComposite(List<Patente> patentes, List<Component> components, string tab)
        {
            foreach (var item in components)
            {
                if (item.ChildrenCount() == 0)
                {
                    // Procesamiento de elemento Leaf (Patente)
                    Patente patente1 = item as Patente;
                    Console.WriteLine($"{tab} Patente: {patente1.FormName}");

                    // Evitar duplicados
                    if (!patentes.Exists(o => o.FormName == patente1.FormName))
                        patentes.Add(patente1);
                }
                else
                {
                    // Procesamiento de elemento Composite (Familia)
                    Familia familia = item as Familia;
                    Console.WriteLine($"{tab} Familia: {familia.Nombre}");

                    // Recursión: Procesar los hijos de esta familia
                    RecorrerComposite(patentes, familia.GetChildrens(), tab + "-");
                }
            }
        }
    }
}
