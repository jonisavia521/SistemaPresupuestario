using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DomainModel.Security.Composite
{
    /// <summary>
    /// PATRÓN COMPOSITE - CLASE COMPOSITE (Composite)
    /// 
    /// Representa un grupo o colección de permisos que puede contener tanto 
    /// permisos individuales (Patentes) como otros grupos (Familias).
    /// 
    /// ANALOGÍA COLOQUIAL:
    /// Una Familia es como una CARPETA en tu sistema de archivos:
    /// - Puede contener archivos (Patentes = permisos individuales)
    /// - Puede contener otras carpetas (Familias = grupos de permisos)
    /// - Tiene un nombre descriptivo
    /// - Se puede organizar jerárquicamente
    /// 
    /// EJEMPLOS PRÁCTICOS EN UN SISTEMA:
    /// 
    /// Familia: "Administración"
    /// ├── Patente: "CrearUsuarios"
    /// ├── Patente: "EliminarUsuarios" 
    /// └── Familia: "Configuración"
    ///     ├── Patente: "CambiarConfiguracion"
    ///     └── Patente: "VerLogs"
    /// 
    /// Familia: "Ventas"
    /// ├── Patente: "CrearPedido"
    /// ├── Patente: "CancelarPedido"
    /// └── Familia: "Reportes"
    ///     ├── Patente: "VerReporteVentas"
    ///     └── Patente: "ExportarReporte"
    /// 
    /// BENEFICIOS:
    /// - Organización lógica de permisos
    /// - Facilita asignación masiva de permisos
    /// - Estructura jerárquica flexible
    /// - Reutilización de grupos de permisos
    /// </summary>
    public class Familia : Component
    {
        /// <summary>
        /// Lista interna que mantiene todos los componentes hijos de esta familia
        /// Puede contener tanto Patentes (permisos individuales) como otras Familias (subgrupos)
        /// 
        /// ANALOGÍA: Como el contenido interno de una carpeta en tu computadora
        /// </summary>
        private List<Component> childrens = new List<Component>();

        /// <summary>
        /// Constructor por defecto que crea una familia vacía
        /// 
        /// EJEMPLO DE USO:
        /// var familiaAdmin = new Familia();
        /// familiaAdmin.Nombre = "Administración";
        /// </summary>
        public Familia()
        {

        }

        /// <summary>
        /// Constructor que crea una familia con un componente inicial y un nombre
        /// 
        /// EJEMPLO DE USO:
        /// var patenteLogin = new Patente { FormName = "frmLogin" };
        /// var familiaSeguridad = new Familia(patenteLogin, "Seguridad");
        /// </summary>
        /// <param name="component">Primer componente a agregar a la familia</param>
        /// <param name="nombre">Nombre descriptivo de la familia</param>
        public Familia(Component component, string nombre)
        {
            childrens.Add(component);
            Nombre = nombre;
        }

        /// <summary>
        /// Nombre descriptivo de la familia de permisos
        /// 
        /// EJEMPLOS COMUNES:
        /// - "Administración"
        /// - "Ventas" 
        /// - "Reportes"
        /// - "Configuración"
        /// - "Usuarios"
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Retorna la lista de componentes hijos de esta familia
        /// 
        /// ANALOGÍA COLOQUIAL:
        /// Como abrir una carpeta y ver todos los archivos y subcarpetas que contiene
        /// 
        /// EJEMPLO DE USO:
        /// foreach (var hijo in familia.GetChildrens())
        /// {
        ///     if (hijo is Patente patente)
        ///         Console.WriteLine($"Permiso: {patente.FormName}");
        ///     else if (hijo is Familia subfamilia)
        ///         Console.WriteLine($"Grupo: {subfamilia.Nombre}");
        /// }
        /// </summary>
        /// <returns>Lista de componentes hijos</returns>
        public List<Component> GetChildrens()
        {
            return childrens;
        }

        /// <summary>
        /// Agrega un componente (Patente o Familia) como hijo de esta familia
        /// 
        /// IMPLEMENTACIÓN DEL PATRÓN COMPOSITE:
        /// Esta clase acepta la operación Add porque es un Composite (puede tener hijos)
        /// 
        /// ANALOGÍA COLOQUIAL:
        /// Como arrastrar un archivo o carpeta dentro de otra carpeta
        /// 
        /// EJEMPLOS DE USO:
        /// // Agregar una patente individual
        /// familia.Add(new Patente { FormName = "frmUsuarios" });
        /// 
        /// // Agregar otra familia (crear jerarquía)
        /// var subfamilia = new Familia();
        /// subfamilia.Nombre = "Reportes";
        /// familia.Add(subfamilia);
        /// 
        /// NOTA IMPORTANTE:
        /// El comentario indica que se debe validar referencias circulares para evitar
        /// que una familia se contenga a sí misma directa o indirectamente
        /// </summary>
        /// <param name="component">Componente a agregar como hijo</param>
        public override void Add(Component component)
        {
            //TODO: Validar que no tenga referencias circulares...
            //Ejemplo de referencia circular: Familia A contiene Familia B, y Familia B contiene Familia A
            childrens.Add(component);
        }

        /// <summary>
        /// Retorna la cantidad de componentes hijos que tiene esta familia
        /// 
        /// IMPLEMENTACIÓN DEL PATRÓN COMPOSITE:
        /// Las familias pueden tener 0 o más hijos (a diferencia de las Patentes que siempre tienen 0)
        /// 
        /// ANALOGÍA COLOQUIAL:
        /// Como contar cuántos elementos hay dentro de una carpeta
        /// 
        /// EJEMPLOS:
        /// - Una familia vacía: retorna 0
        /// - Familia con 3 patentes y 2 subfamilias: retorna 5
        /// </summary>
        /// <returns>Número de componentes hijos</returns>
        public override int ChildrenCount()
        {
            return childrens.Count;
        }

        /// <summary>
        /// Remueve un componente específico de los hijos de esta familia
        /// 
        /// IMPLEMENTACIÓN DEL PATRÓN COMPOSITE:
        /// Las familias aceptan la operación Remove porque pueden contener hijos
        /// 
        /// ANALOGÍA COLOQUIAL:
        /// Como eliminar un archivo o carpeta de dentro de otra carpeta
        /// 
        /// NOTA TÉCNICA:
        /// Utiliza RemoveAll con comparación de IdComponent para remover todas las
        /// instancias que tengan el mismo ID (aunque debería haber solo una)
        /// 
        /// EJEMPLO DE USO:
        /// var patente = new Patente { IdComponent = Guid.NewGuid() };
        /// familia.Add(patente);
        /// // ... más tarde ...
        /// familia.Remove(patente); // Remueve la patente de la familia
        /// </summary>
        /// <param name="component">Componente a remover</param>
        public override void Remove(Component component)
        {
            childrens.RemoveAll(o => o.IdComponent == component.IdComponent);
        }

        /// <summary>
        /// Agrega múltiples componentes a esta familia de una sola vez
        /// 
        /// IMPLEMENTACIÓN DEL PATRÓN COMPOSITE:
        /// Las familias aceptan esta operación porque pueden contener múltiples hijos
        /// 
        /// ANALOGÍA COLOQUIAL:
        /// Como copiar múltiples archivos y carpetas dentro de una carpeta destino
        /// 
        /// EJEMPLO DE USO:
        /// var permisos = new List<Component>
        /// {
        ///     new Patente { FormName = "frmUsuarios" },
        ///     new Patente { FormName = "frmReportes" },
        ///     new Familia { Nombre = "Configuración" }
        /// };
        /// familia.Set(permisos);
        /// 
        /// NOTA: Esta implementación AGREGA a los hijos existentes, no los reemplaza
        /// </summary>
        /// <param name="components">Lista de componentes a agregar</param>
        public override void Set(List<Component> components)
        {
            this.childrens.AddRange(components);
        }
    }
}
