using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DomainModel.Security.Composite
{
    /// <summary>
    /// PATRÓN COMPOSITE - CLASE COMPOSITE (Contenedor)
    /// 
    /// Representa un grupo de permisos que puede contener tanto elementos individuales (Patentes)
    /// como otros grupos (Familias), formando una estructura jerárquica de permisos.
    /// 
    /// ANALOGÍA COLOQUIAL:
    /// Una Familia es como una CARPETA en tu sistema de archivos:
    /// - Puede contener archivos individuales (Patentes)
    /// - Puede contener otras carpetas (Familias)
    /// - Puede estar vacía o tener cualquier número de elementos
    /// - Organiza permisos de forma jerárquica para facilitar su gestión
    /// 
    /// EN NUESTRO SISTEMA DE PERMISOS:
    /// Las familias permiten agrupar permisos relacionados, facilitando:
    /// - Asignación masiva de permisos a usuarios (un grupo completo de una vez)
    /// - Organización lógica del sistema de seguridad
    /// - Reutilización de conjuntos de permisos entre usuarios
    /// - Jerarquías de permisos de cualquier profundidad
    /// 
    /// EJEMPLOS PRÁCTICOS EN EL SISTEMA:
    /// 
    /// Familia "Administración":
    /// ├── Patente: "frmUsuarios" (acceso al formulario de usuarios)
    /// ├── Patente: "frmBackup" (acceso al formulario de respaldos)
    /// └── Familia: "Configuración Avanzada"
    ///     ├── Patente: "frmConfiguracionGeneral"
    ///     └── Patente: "frmActualizarPadronArba"
    /// 
    /// Familia "Ventas":
    /// ├── Patente: "frmPresupuesto" (acceso a cotizaciones)
    /// ├── Patente: "frmFacturar" (acceso a facturación)
    /// └── Familia: "Maestros de Venta"
    ///     ├── Patente: "frmClientes"
    ///     ├── Patente: "frmProductos"
    ///     └── Patente: "frmListaPrecios"
    /// 
    /// VENTAJAS DEL PATRÓN COMPOSITE:
    /// 1. Flexibilidad: Estructura de permisos adaptable a cualquier necesidad
    /// 2. Reusabilidad: Las familias se pueden asignar a múltiples usuarios
    /// 3. Mantenimiento: Cambiar permisos de una familia afecta a todos sus usuarios
    /// 4. Escalabilidad: Soporta jerarquías de cualquier profundidad sin cambios de código
    /// </summary>
    public class Familia : Component
    {
        /// <summary>
        /// Colección privada de componentes hijos (Patentes y/o Familias).
        /// Representa el contenido de esta carpeta de permisos.
        /// </summary>
        private List<Component> childrens = new List<Component>();

        /// <summary>
        /// Constructor por defecto para crear una familia vacía.
        /// Usado cuando se creará una familia y luego se agregarán sus componentes.
        /// </summary>
        public Familia()
        {
        }

        /// <summary>
        /// Constructor para crear una familia con un primer componente inicial.
        /// 
        /// USO TÍPICO:
        /// Cuando se crea una familia y ya se conoce al menos un permiso que debe contener.
        /// </summary>
        /// <param name="component">Primer componente (Patente o Familia) que contendrá</param>
        /// <param name="nombre">Nombre descriptivo de la familia</param>
        /// <example>
        /// <code>
        /// var patenteUsuarios = new Patente { FormName = "frmUsuarios" };
        /// var familiaAdmin = new Familia(patenteUsuarios, "Administración");
        /// </code>
        /// </example>
        public Familia(Component component, string nombre)
        {
            childrens.Add(component);
            Nombre = nombre;
        }

        /// <summary>
        /// Nombre descriptivo de la familia de permisos.
        /// 
        /// PROPÓSITO:
        /// Identifica de forma clara y comprensible el propósito de este grupo de permisos.
        /// 
        /// EJEMPLOS COMUNES:
        /// - "Administración" - Agrupa permisos de administración del sistema
        /// - "Ventas" - Agrupa permisos relacionados con el proceso de ventas
        /// - "Maestros" - Agrupa permisos de ABM de datos maestros
        /// - "Reportes" - Agrupa permisos de visualización de informes
        /// - "Configuración" - Agrupa permisos de configuración del sistema
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Nombre del formulario asociado a esta familia (opcional).
        /// 
        /// PROPÓSITO:
        /// Algunas familias pueden representar acceso a un formulario principal
        /// que a su vez controla acceso a subformularios o funcionalidades.
        /// 
        /// EJEMPLO:
        /// Una familia "Ventas" podría tener FormName = "frmModuloVentas"
        /// y contener patentes para subfuncionalidades específicas.
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// Retorna la lista de componentes hijos de esta familia.
        /// 
        /// PROPÓSITO:
        /// Permite recorrer recursivamente la estructura de permisos para:
        /// - Mostrar el árbol jerárquico en la UI
        /// - Obtener todas las patentes de un usuario
        /// - Validar permisos de acceso
        /// 
        /// USO EN EL SISTEMA:
        /// El método Usuario.GetPatentesAll() utiliza este método para recorrer
        /// recursivamente todas las familias y extraer las patentes.
        /// </summary>
        /// <returns>Lista de componentes (Patentes y/o Familias) contenidos en esta familia</returns>
        public List<Component> GetChildrens()
        {
            return childrens;
        }

        /// <summary>
        /// IMPLEMENTACIÓN DEL PATRÓN COMPOSITE - Operación Add
        /// 
        /// Agrega un componente (Patente o Familia) a esta familia.
        /// 
        /// ANALOGÍA:
        /// Como agregar un archivo o subcarpeta dentro de una carpeta.
        /// La carpeta no necesita saber si está agregando un archivo o una subcarpeta,
        /// simplemente lo agrega a su contenido.
        /// 
        /// VALIDACIÓN PENDIENTE (TODO):
        /// Actualmente no valida referencias circulares, lo cual podría causar loops infinitos.
        /// Una referencia circular ocurre cuando:
        /// - Familia A contiene Familia B
        /// - Familia B contiene Familia A (directa o indirectamente)
        /// 
        /// EJEMPLO DE USO:
        /// <code>
        /// var familiaAdmin = new Familia { Nombre = "Administración" };
        /// var patenteUsuarios = new Patente { FormName = "frmUsuarios" };
        /// familiaAdmin.Add(patenteUsuarios);
        /// 
        /// var familiaConfig = new Familia { Nombre = "Configuración" };
        /// familiaAdmin.Add(familiaConfig); // Agregar una subfamilia
        /// </code>
        /// 
        /// ADVERTENCIA:
        /// Evitar referencias circulares manualmente hasta que se implemente la validación.
        /// </summary>
        /// <param name="component">Componente a agregar (Patente o Familia)</param>
        public override void Add(Component component)
        {
            // TODO: Validar que no tenga referencias circulares
            // Ejemplo de referencia circular: Familia A contiene Familia B, y Familia B contiene Familia A
            childrens.Add(component);
        }

        /// <summary>
        /// IMPLEMENTACIÓN DEL PATRÓN COMPOSITE - Conteo de hijos
        /// 
        /// Retorna la cantidad de componentes que contiene esta familia.
        /// 
        /// PROPÓSITO:
        /// Permite determinar si una familia está vacía o cuántos elementos contiene.
        /// Usado en algoritmos de recorrido para optimizar el procesamiento.
        /// 
        /// USO EN RECORRIDO RECURSIVO:
        /// El método Usuario.RecorrerComposite() usa este valor para determinar
        /// si debe seguir profundizando en la jerarquía:
        /// - ChildrenCount() == 0 → Es una Patente (elemento terminal)
        /// - ChildrenCount() > 0 → Es una Familia (seguir recorriendo)
        /// 
        /// EJEMPLO:
        /// <code>
        /// if (component.ChildrenCount() == 0)
        /// {
        ///     // Es una Patente, procesarla
        ///     var patente = component as Patente;
        ///     procesarPatente(patente);
        /// }
        /// else
        /// {
        ///     // Es una Familia, recorrer sus hijos
        ///     var familia = component as Familia;
        ///     RecorrerHijos(familia.GetChildrens());
        /// }
        /// </code>
        /// </summary>
        /// <returns>Número de componentes hijos (0 si está vacía, >0 si contiene elementos)</returns>
        public override int ChildrenCount()
        {
            return childrens.Count;
        }

        /// <summary>
        /// IMPLEMENTACIÓN DEL PATRÓN COMPOSITE - Operación Remove
        /// 
        /// Remueve un componente específico de esta familia.
        /// 
        /// ANALOGÍA:
        /// Como eliminar un archivo o subcarpeta de dentro de una carpeta.
        /// 
        /// MECANISMO DE BÚSQUEDA:
        /// Busca el componente por su IdComponent (GUID único) y lo elimina.
        /// Si no existe, no hace nada (operación segura).
        /// 
        /// CASO DE USO:
        /// Cuando se revoca un permiso específico de una familia sin eliminar
        /// la familia completa.
        /// 
        /// EJEMPLO:
        /// <code>
        /// var familiaAdmin = ObtenerFamilia("Administración");
        /// var patenteBackup = ObtenerPatente("frmBackup");
        /// 
        /// // Remover acceso al formulario de backup
        /// familiaAdmin.Remove(patenteBackup);
        /// </code>
        /// </summary>
        /// <param name="component">Componente a remover (se busca por IdComponent)</param>
        public override void Remove(Component component)
        {
            childrens.RemoveAll(o => o.IdComponent == component.IdComponent);
        }

        /// <summary>
        /// IMPLEMENTACIÓN DEL PATRÓN COMPOSITE - Operación Set
        /// 
        /// Agrega múltiples componentes a esta familia de una sola vez.
        /// 
        /// ANALOGÍA:
        /// Como copiar un grupo de archivos y carpetas dentro de una carpeta de destino.
        /// 
        /// COMPORTAMIENTO:
        /// No reemplaza los elementos existentes, sino que agrega los nuevos a la colección.
        /// Si se desea reemplazar completamente, primero limpiar con childrens.Clear().
        /// 
        /// CASO DE USO:
        /// Cuando se carga la estructura de permisos desde la base de datos
        /// y se necesita establecer todos los hijos de una familia de una vez.
        /// 
        /// EJEMPLO:
        /// <code>
        /// var familiaVentas = new Familia { Nombre = "Ventas" };
        /// 
        /// var permisos = new List&lt;Component&gt;
        /// {
        ///     new Patente { FormName = "frmPresupuesto" },
        ///     new Patente { FormName = "frmFacturar" },
        ///     new Patente { FormName = "frmClientes" }
        /// };
        /// 
        /// familiaVentas.Set(permisos);
        /// </code>
        /// </summary>
        /// <param name="components">Lista de componentes a agregar a esta familia</param>
        public override void Set(List<Component> components)
        {
            this.childrens.AddRange(components);
        }
    }
}
