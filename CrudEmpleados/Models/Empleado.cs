using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrudEmpleados.Models
{
    public class Empleado
    {
        //creamos los campos de nuestra tabla empleado usaremos firt code//

        [Key]
        [Display(Name ="Código")]
        public int Id { get; set; }

        [Required(ErrorMessage ="El nombre es obligatorio")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El lugar de nacimiento es obligatorio")]
        [Display(Name ="Lugar de nacimiento")]
        public string LugarNac { get; set; }

        [Required(ErrorMessage = "Fecha nacimiento es obligatorio")]
        [Display(Name = "Fecha de nacimiento")]
        public string FechaNac { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [Display(Name ="Dirección")]
        public string Direccion { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Foto { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Display(Name ="Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Puesto laboral es obligatorio")]
        public string Puesto { get; set; }

        [Required(ErrorMessage = "Seleccione el estado del empleado")]
        public int Estado { get; set; }
    }
}
