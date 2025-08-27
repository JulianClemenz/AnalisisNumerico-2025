using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DatosMetodoAbiertoDTO
    {
        [Required(ErrorMessage = "funcion no puede ser null")]
        public string funcion {  get; set; }

        [Required(ErrorMessage ="tolerancia no puede ser null")]
        public double tolerancia { get; set; }

        [Required(ErrorMessage ="iteraciones no puede ser null")]
        public int iteraciones { get; set; }

        [Required(ErrorMessage ="el X inicial no puede ser null")]
        public double Xini {  get; set; }


    }
}
