using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class MetodoSecanteDTO
    {
        [Required(ErrorMessage = "funcion no puede ser null")]
        public string funcion { get; set; }

        [Required(ErrorMessage = "tolerancia no puede ser null")]
        public double tolerancia { get; set; }

        [Required(ErrorMessage = "iteraciones no puede ser null")]
        public int iteraciones { get; set; }

        [Required(ErrorMessage = "el X1 inicial no puede ser null")]
        public double Xini1 { get; set; }

        [Required(ErrorMessage = "el X2 inicial no puede ser null")]
        public double Xini2 { get; set; }
    }
}