using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class DatosMetodoCerradoDTO
    {
        [Required(ErrorMessage = "La función es requerida.")]
        public string funcion { get; set; }
        [Required(ErrorMessage = "El valor de Xi es requerido.")]
        public double Xi { get; set; }
        [Required(ErrorMessage = "El valor de Xd es requerido.")]
        public double Xd { get; set; }
        [Required(ErrorMessage = "El número de iteraciones es requerido.")]
        public int iteraciones { get; set; }
        [Required(ErrorMessage = "La tolerancia es requerida.")]
        public double tolerancia { get; set; }
        [Required(ErrorMessage = "El método es requerido.")]
        public bool metodo { get; set; } // true para regla falsa, false para bisección

    }
}
