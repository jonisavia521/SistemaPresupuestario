using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Shared
{
    /// <summary>
    /// Configuración para el selector genérico
    /// Define qué columnas mostrar y cómo filtrar
    /// </summary>
    /// <typeparam name="T">Tipo de objeto a seleccionar</typeparam>
    public class SelectorConfig<T>
    {
        /// <summary>
        /// Título de la ventana
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Lista de objetos a mostrar
        /// </summary>
        public IEnumerable<T> Datos { get; set; }

        /// <summary>
        /// Definición de columnas a mostrar
        /// </summary>
        public List<ColumnaConfig> Columnas { get; set; }

        /// <summary>
        /// Función de filtrado personalizada
        /// Recibe el texto de búsqueda y el objeto, retorna true si coincide
        /// </summary>
        public Func<string, T, bool> FuncionFiltro { get; set; }

        /// <summary>
        /// Texto del placeholder para el cuadro de búsqueda
        /// </summary>
        public string PlaceholderBusqueda { get; set; }

        /// <summary>
        /// Permite selección múltiple
        /// </summary>
        public bool PermitirSeleccionMultiple { get; set; }

        public SelectorConfig()
        {
            Columnas = new List<ColumnaConfig>();
            PlaceholderBusqueda = "Buscar...";
            PermitirSeleccionMultiple = false;
        }
    }

    /// <summary>
    /// Configuración de una columna del DataGridView
    /// </summary>
    public class ColumnaConfig
    {
        /// <summary>
        /// Nombre de la propiedad del objeto a mostrar
        /// </summary>
        public string NombrePropiedad { get; set; }

        /// <summary>
        /// Título de la columna
        /// </summary>
        public string TituloColumna { get; set; }

        /// <summary>
        /// Ancho de la columna
        /// </summary>
        public int Ancho { get; set; }

        /// <summary>
        /// Si la columna es visible
        /// </summary>
        public bool Visible { get; set; }

        public ColumnaConfig()
        {
            Visible = true;
            Ancho = 100;
        }
    }
}
