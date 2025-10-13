using Entity;
using Microsoft.AspNetCore.Mvc;
using TP3;

namespace RegresionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegresionLinealController : ControllerBase
    {
        [HttpPost]
        public IActionResult RegresionLineal(DatoEntradaLineal entrada)
        {

            if (entrada == null || entrada.puntosCargados == null)//verificamos si los campos de entrada no son nulos
            {
                return BadRequest("Entrada inválida:Campos nulos");
            }

            double tolerancia = entrada.Tolerancia;
            List<Double[]> puntosCargados = entrada.puntosCargados;

            double SumaX = 0;
            double SumaY = 0;
            double SumaXY = 0;
            double SumaX2 = 0;
            double a1 = 0;
            double a0 = 0;
            double St = 0;
            double Sr = 0;
            int n = puntosCargados.Count;

            if (puntosCargados.Count < 2)//verificamos que existan al menos dos puntos
            {
                return BadRequest("Se requieren al menos 2 puntos.");
            }

            foreach (var p in puntosCargados)//se verifica q cada punto tengo un X y un Y
            {
                if (p == null || p.Length < 2)
                {
                    return BadRequest("Cada punto debe tener al menos [x,y].");
                }
            }

            double r = 0;
            string Funcion;

            //Calcular la sumatoria de X (SumX).
            //Calcular la sumatoria de Y (SumY)
            //Calcular la sumatoria de XY (SumXY).
            //Calcular la sumatoria de X² (SumX2)
            for (int i = 0; i < puntosCargados.Count; ++i)
            {
                SumaX += puntosCargados[i][0];
                SumaY += puntosCargados[i][1];
                SumaXY += puntosCargados[i][0]* puntosCargados[i][1];
                SumaX2 += puntosCargados[i][0]* puntosCargados[i][0];
            }

            if (n * SumaX2 - Math.Pow(SumaX, 2) == 0)
            {
                return BadRequest("No se puede calcular la regresión con  dicha lista de puntos: división por cero");
            }

            //Calcular a1.
            //Calcular a0
            a1 = (n * SumaXY - SumaX *  SumaY)/(n * SumaX2 - Math.Pow(SumaX,2));
            a0 = (SumaY / n) - a1 * (SumaX/n);

            //Calcular Sr y St
            for (int j = 0; j < puntosCargados.Count; ++j)
            {
                St += Math.Pow((SumaY / n - puntosCargados[j][1]), 2);
                Sr += Math.Pow((a1 * puntosCargados[j][0] + a0 - puntosCargados[j][1]), 2);
            }

            //calcular el coeficiente de correlación r.
            r = Math.Sqrt((St-Sr)/St) * 100;

            Funcion = $"y = {a1:F2} x + {a0:F2}";

            DatoSalidaGeneral salida = new DatoSalidaGeneral()
            {
                Funcion = Funcion,
                PorcCorrelacion = r,
                EfectAjuste = r > tolerancia
            };
           
            return Ok(salida);
        }
    }
}
