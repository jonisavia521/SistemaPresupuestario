using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DomainModel.Security.Composite
{
    /// <summary>
    /// PATRÓN COMPOSITE - COMPONENTE ABSTRACTO (Component)
    /// 
    /// Esta clase implementa el patrón de diseño Composite, que permite tratar objetos individuales 
    /// y composiciones de objetos de manera uniforme.
    /// 
    /// ANALOGÍA COLOQUIAL:
    /// Piensa en el sistema de archivos de tu computadora:
    /// - Una CARPETA puede contener archivos Y otras carpetas
    /// - Un ARCHIVO es un elemento individual que no contiene nada más
    /// - Tanto carpetas como archivos son "elementos del sistema de archivos"
    /// - Puedes realizar operaciones similares en ambos (copiar, mover, eliminar)
    /// 
    /// EN NUESTRO SISTEMA DE PERMISOS:
    /// - Component = "Elemento del sistema de permisos" (como archivo o carpeta)
    /// - Familia = "Carpeta de permisos" (puede contener otras familias o patentes)
    /// - Patente = "Archivo de permiso" (permiso individual, no contiene otros)
    /// 
    /// BENEFICIOS DEL PATRÓN:
    /// 1. Simplicidad: El cliente trata igual a elementos simples y compuestos
    /// 2. Extensibilidad: Fácil agregar nuevos tipos de componentes
    /// 3. Flexibilidad: Estructuras jerárquicas de cualquier nivel
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Identificador único del componente en el sistema
        /// Usado para distinguir entre diferentes permisos o familias
        /// </summary>
        public Guid IdComponent { get; set; }

        /// <summary>
        /// Agrega un componente hijo a este elemento
        /// 
        /// COMPORTAMIENTO POR TIPO:
        /// - Familia: Agrega el componente a su lista de hijos
        /// - Patente: Lanza excepción (no puede tener hijos)
        /// 
        /// EJEMPLO COLOQUIAL:
        /// Como agregar un archivo o subcarpeta dentro de una carpeta
        /// </summary>
        /// <param name="component">El componente a agregar como hijo</param>
        public abstract void Add(Component component);

        /// <summary>
        /// Remueve un componente hijo de este elemento
        /// 
        /// COMPORTAMIENTO POR TIPO:
        /// - Familia: Remueve el componente de su lista de hijos
        /// - Patente: Lanza excepción (no tiene hijos para remover)
        /// 
        /// EJEMPLO COLOQUIAL:
        /// Como eliminar un archivo o subcarpeta de dentro de una carpeta
        /// </summary>
        /// <param name="component">El componente a remover</param>
        public abstract void Remove(Component component);

        /// <summary>
        /// Retorna la cantidad de componentes hijos que tiene este elemento
        /// 
        /// COMPORTAMIENTO POR TIPO:
        /// - Patente: Siempre retorna 0 (no tiene hijos)
        /// - Familia: Retorna el número de elementos que contiene (>= 0)
        /// 
        /// EJEMPLO COLOQUIAL:
        /// Como contar cuántos archivos y subcarpetas hay dentro de una carpeta
        /// Un archivo siempre tiene 0 elementos dentro
        /// </summary>
        /// <returns>Número de componentes hijos</returns>
        public abstract int ChildrenCount();

        /// <summary>
        /// Establece una lista completa de componentes hijos de una vez
        /// 
        /// COMPORTAMIENTO POR TIPO:
        /// - Familia: Agrega todos los componentes de la lista a sus hijos
        /// - Patente: Lanza excepción (no puede tener hijos)
        /// 
        /// EJEMPLO COLOQUIAL:
        /// Como copiar múltiples archivos y carpetas dentro de una carpeta de destino
        /// </summary>
        /// <param name="components">Lista de componentes a establecer como hijos</param>
        public abstract void Set(List<Component> components);
    }
}
