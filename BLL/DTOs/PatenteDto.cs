using System;
using System.ComponentModel;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para representar patentes (permisos individuales)
    /// Diferencia entre permisos directos y heredados
    /// </summary>
    public class PatenteDto
    {
        /// <summary>
        /// Identificador único de la patente
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre descriptivo de la patente
        /// </summary>
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        /// <summary>
        /// Vista o formulario al que da acceso esta patente
        /// Usado para control de acceso en la aplicación
        /// </summary>
        [DisplayName("Vista")]
        public string Vista { get; set; }

        /// <summary>
        /// Indica si este permiso está asignado directamente al usuario
        /// </summary>
        [Browsable(false)]
        public bool EsDirecta { get; set; }

        /// <summary>
        /// Indica si este permiso está heredado a través de una familia
        /// </summary>
        [Browsable(false)]
        public bool EsHeredada { get; set; }

        /// <summary>
        /// Origen del permiso (para trazabilidad)
        /// Ejemplo: "Directo", "Familia: Administración", "Familia: Administración > Configuración"
        /// </summary>
        [DisplayName("Origen")]
        public string Origen { get; set; }

        /// <summary>
        /// Timestamp para control de concurrencia
        /// </summary>
        [Browsable(false)]
        public string TimestampBase64 { get; set; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public PatenteDto()
        {
            Origen = "Directo";
        }

        /// <summary>
        /// Constructor para patente directa
        /// </summary>
        public PatenteDto(Guid id, string nombre, string vista) : this()
        {
            Id = id;
            Nombre = nombre;
            Vista = vista;
            EsDirecta = true;
            EsHeredada = false;
        }

        /// <summary>
        /// Constructor para patente heredada
        /// </summary>
        public PatenteDto(Guid id, string nombre, string vista, string origenFamilia) : this()
        {
            Id = id;
            Nombre = nombre;
            Vista = vista;
            EsDirecta = false;
            EsHeredada = true;
            Origen = $"Familia: {origenFamilia}";
        }

        /// <summary>
        /// Indica si esta patente es efectiva (directa o heredada)
        /// </summary>
        public bool EsEfectiva => EsDirecta || EsHeredada;

        public override bool Equals(object obj)
        {
            if (obj is PatenteDto other)
            {
                return Id.Equals(other.Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}