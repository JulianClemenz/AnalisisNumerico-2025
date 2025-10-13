using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class DatoSalidaGeneral
    {
        public string Funcion {  get; set; }
        public double PorcCorrelacion { get; set; }//coeficiente de correlacion "r"
        public bool EfectAjuste { get; set; }// mensaje de si es aceptable o no el ajuste en relacion a la tolerancia
    }
}
