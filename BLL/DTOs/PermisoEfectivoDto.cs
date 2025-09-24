using System;
using System.ComponentModel;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para representar permisos efectivos consolidados de un usuario
    /// Muestra la vista unificada de todos los permisos (directos + heredados)
    /// </summary>
    public class PermisoEfectivoDto
    {
        /// <summary>
        /// Tipo de permiso
        /// </summary>
        public enum TipoPermiso
        {
            Patente,
            Familia
        }

        /// <summary>
        /// Origen del permiso
        /// </summary>
        public enum OrigenPermiso
        {
            Directo,
            Heredado
        }

        /// <summary>
        /// Identificador único del permiso
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tipo de elemento (Patente o Familia)
        /// </summary>
        [DisplayName("Tipo")]
        public TipoPermiso Tipo { get; set; }

        /// <summary>
        /// Nombre descriptivo del permiso
        /// </summary>
        [DisplayName("Permiso")]
        public string Nombre { get; set; }

        /// <summary>
        /// Origen del permiso (Directo o Heredado)
        /// </summary>
        [DisplayName("Origen")]
        public OrigenPermiso Origen { get; set; }

        /// <summary>
        /// Descripción detallada del origen (nombre de familia padre si es heredado)
        /// </summary>
        [DisplayName("Detalle Origen")]
        public string DetalleOrigen { get; set; }

        /// <summary>
        /// Vista asociada (solo para patentes)
        /// </summary>
        [DisplayName("Vista")]
        public string Vista { get; set; }

        /// <summary>
        /// Nivel de jerarquía (para indentación visual)
        /// </summary>
        [Browsable(false)]
        public int Nivel { get; set; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public PermisoEfectivoDto()
        {
        }

        /// <summary>
        /// Constructor para permiso directo
        /// </summary>
        public PermisoEfectivoDto(Guid id, TipoPermiso tipo, string nombre, string vista = null)
        {
            Id = id;
            Tipo = tipo;
            Nombre = nombre;
            Vista = vista;
            Origen = OrigenPermiso.Directo;
            DetalleOrigen = "Asignación directa";
            Nivel = 0;
        }

        /// <summary>
        /// Constructor para permiso heredado
        /// </summary>
        public PermisoEfectivoDto(Guid id, TipoPermiso tipo, string nombre, string origenFamilia, string vista = null, int nivel = 1)
        {
            Id = id;
            Tipo = tipo;
            Nombre = nombre;
            Vista = vista;
            Origen = OrigenPermiso.Heredado;
            DetalleOrigen = $"Heredado de: {origenFamilia}";
            Nivel = nivel;
        }

        /// <summary>
        /// Representa el tipo como string para visualización
        /// </summary>
        public string TipoTexto => Tipo == TipoPermiso.Patente ? "Patente" : "Familia";

        /// <summary>
        /// Representa el origen como string para visualización
        /// </summary>
        public string OrigenTexto => Origen == OrigenPermiso.Directo ? "Directo" : "Heredado";

        public override bool Equals(object obj)
        {
            if (obj is PermisoEfectivoDto other)
            {
                return Id.Equals(other.Id) && Tipo.Equals(other.Tipo);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Tipo);
        }
    }
}