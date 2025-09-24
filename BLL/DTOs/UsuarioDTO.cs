using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class UsuarioDTO
    {
        [Browsable(false)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Usuario es requerido")]
        public string Usuario { get; set; }

        [Required(AllowEmptyStrings = true,ErrorMessage = "La clave es obligatorio")]
        public string Clave { get; set; }

        //[Browsable(false)] //Atributo que no se muestra como propiedad en objetos de ventanas...
        //[Required(ErrorMessage = "Nombre es requerido")]
        //[StringLength(maximumLength: 8, MinimumLength = 6, ErrorMessage = "El dni no contiene el formato necesario: 6-8")]
    }
}