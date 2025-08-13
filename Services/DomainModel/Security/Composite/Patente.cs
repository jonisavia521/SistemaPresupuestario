using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DomainModel.Security.Composite
{
    /// <summary>
    /// PATRÓN COMPOSITE - CLASE LEAF (Hoja)
    /// 
    /// Representa un permiso individual y específico en el sistema. 
    /// Es un elemento "hoja" que no puede contener otros elementos.
    /// 
    /// ANALOGÍA COLOQUIAL:
    /// Una Patente es como un ARCHIVO en tu sistema de archivos:
    /// - Es un elemento individual y específico
    /// - NO puede contener otros archivos o carpetas dentro de él
    /// - Tiene propiedades específicas (nombre, tipo, etc.)
    /// - Representa una "unidad atómica" de funcionalidad
    /// 
    /// EN NUESTRO SISTEMA:
    /// Una patente representa un permiso específico como:
    /// - Permiso para acceder a un formulario específico
    /// - Permiso para usar una funcionalidad específica
    /// - Permiso para acceder a un elemento de menú específico
    /// 
    /// EJEMPLOS PRÁCTICOS:
    /// - FormName: "frmUsuarios" → Permiso para acceder al formulario de usuarios
    /// - FormName: "frmReportes" → Permiso para acceder al formulario de reportes  
    /// - MenuItemName: "mnuCrearUsuario" → Permiso para usar el menú "Crear Usuario"
    /// - MenuItemName: "mnuEliminarUsuario" → Permiso para usar el menú "Eliminar Usuario"
    /// 
    /// CARACTERÍSTICAS DEL PATRÓN LEAF:
    /// - No tiene hijos (ChildrenCount siempre retorna 0)
    /// - Las operaciones Add, Remove, Set lanzan excepciones
    /// - Implementa el comportamiento específico del elemento
    /// </summary>
    public class Patente : Component
    {
        /// <summary>
        /// Nombre del formulario al cual esta patente da acceso
        /// 
        /// PROPÓSITO:
        /// Identifica qué formulario específico de la aplicación puede ser accedido
        /// con este permiso
        /// 
        /// EJEMPLOS COMUNES:
        /// - "frmLogin" → Formulario de inicio de sesión
        /// - "frmUsuarios" → Formulario de gestión de usuarios
        /// - "frmReportes" → Formulario de reportes
        /// - "frmConfiguracion" → Formulario de configuración
        /// - "frmVentas" → Formulario de ventas
        /// 
        /// USO EN LA APLICACIÓN:
        /// Cuando un usuario intenta acceder a un formulario, el sistema verifica
        /// si tiene una patente con el FormName correspondiente
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// Nombre del elemento de menú al cual esta patente da acceso
        /// 
        /// PROPÓSITO:
        /// Identifica qué opción específica del menú puede ser utilizada
        /// con este permiso
        /// 
        /// EJEMPLOS COMUNES:
        /// - "mnuCrearUsuario" → Opción de menú "Crear Usuario"
        /// - "mnuEliminarUsuario" → Opción de menú "Eliminar Usuario"
        /// - "mnuGenerarReporte" → Opción de menú "Generar Reporte"
        /// - "mnuConfiguracion" → Opción de menú "Configuración"
        /// - "mnuExportar" → Opción de menú "Exportar"
        /// 
        /// USO EN LA APLICACIÓN:
        /// El sistema puede habilitar/deshabilitar opciones de menú basándose
        /// en las patentes que tiene el usuario
        /// </summary>
        public string MenuItemName { get; set; }

        /// <summary>
        /// IMPLEMENTACIÓN DEL PATRÓN LEAF - Operación Add
        /// 
        /// Las patentes son elementos "hoja" (leaf) en el patrón Composite,
        /// lo que significa que NO pueden contener elementos hijos.
        /// 
        /// ANALOGÍA COLOQUIAL:
        /// Es como tratar de meter archivos dentro de un archivo de texto.
        /// Un archivo no puede contener otros archivos, solo una carpeta puede.
        /// 
        /// POR QUÉ LANZA EXCEPCIÓN:
        /// - Mantiene la integridad del patrón Composite
        /// - Previene uso incorrecto de la clase
        /// - Hace explícito que las patentes son elementos terminales
        /// 
        /// ALTERNATIVA DE DISEÑO:
        /// En lugar de lanzar excepción, se podría implementar como operación vacía,
        /// pero lanzar excepción es más claro sobre la intención del diseño
        /// </summary>
        /// <param name="component">Componente que se intenta agregar (será rechazado)</param>
        /// <exception cref="Exception">Siempre lanza excepción porque las patentes no pueden tener hijos</exception>
        public override void Add(Component component)
        {
            throw new Exception("No se pueden agregar elementos sobre primitivos");
        }

        /// <summary>
        /// IMPLEMENTACIÓN DEL PATRÓN LEAF - Conteo de hijos
        /// 
        /// Las patentes siempre retornan 0 porque son elementos "hoja" que no
        /// pueden contener otros elementos.
        /// 
        /// ANALOGÍA COLOQUIAL:
        /// Como preguntar cuántos archivos hay dentro de un archivo de texto.
        /// La respuesta siempre es 0 porque los archivos no contienen otros archivos.
        /// 
        /// USO EN EL SISTEMA:
        /// Este método es utilizado por algoritmos que recorren la estructura
        /// jerárquica para determinar si deben seguir profundizando o si
        /// llegaron a un elemento terminal.
        /// 
        /// Ejemplo en Usuario.cs:
        /// if (item.ChildrenCount() == 0) // Es una patente
        /// {
        ///     // Procesar como patente individual
        /// }
        /// else // Es una familia
        /// {
        ///     // Seguir recorriendo sus hijos
        /// }
        /// </summary>
        /// <returns>Siempre retorna 0 porque las patentes no tienen hijos</returns>
        public override int ChildrenCount()
        {
            return 0;
        }

        /// <summary>
        /// IMPLEMENTACIÓN DEL PATRÓN LEAF - Operación Remove
        /// 
        /// Las patentes no pueden tener hijos, por lo tanto no hay nada que remover.
        /// 
        /// ANALOGÍA COLOQUIAL:
        /// Es como tratar de eliminar archivos de dentro de un archivo de texto.
        /// No tiene sentido porque los archivos no contienen otros archivos.
        /// 
        /// CONSISTENCIA DEL PATRÓN:
        /// Aunque podría implementarse como operación vacía, lanzar excepción
        /// mantiene consistencia con Add() y hace explícito que la operación
        /// no es válida para este tipo de componente.
        /// </summary>
        /// <param name="component">Componente que se intenta remover (será rechazado)</param>
        /// <exception cref="Exception">Siempre lanza excepción porque las patentes no tienen hijos</exception>
        public override void Remove(Component component)
        {
            throw new Exception("No se pueden quitar elementos sobre primitivos");
        }

        /// <summary>
        /// IMPLEMENTACIÓN DEL PATRÓN LEAF - Operación Set
        /// 
        /// Las patentes no pueden contener conjuntos de elementos porque son
        /// elementos terminales (hojas) del árbol de permisos.
        /// 
        /// ANALOGÍA COLOQUIAL:
        /// Es como tratar de copiar múltiples archivos dentro de un archivo de texto.
        /// Los archivos no pueden contener otros archivos, solo las carpetas pueden.
        /// 
        /// PROPÓSITO DE LA EXCEPCIÓN:
        /// - Evita uso incorrecto de la API
        /// - Mantiene clara la semántica del patrón Composite
        /// - Fuerza al desarrollador a usar correctamente las familias para agrupar
        /// 
        /// USO CORRECTO:
        /// Si necesitas agrupar patentes, crea una Familia y usa su método Set():
        /// var familia = new Familia();
        /// familia.Set(listaDePatentes);
        /// </summary>
        /// <param name="components">Lista de componentes que se intenta establecer (será rechazada)</param>
        /// <exception cref="Exception">Siempre lanza excepción porque las patentes no pueden contener elementos</exception>
        public override void Set(List<Component> components)
        {
            throw new Exception("No se pueden cargar un grupo de elementos sobre primitivos");
        }
    }
}
