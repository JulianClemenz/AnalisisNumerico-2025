using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DTO;
using Calculus;

namespace ApiREST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecanteController : ControllerBase
    {
        private Calculo analizadorDeFunciones = new Calculo();

        [HttpPost]
        public IActionResult PostSecante([FromBody] MetodoSecanteDTO datos)
        {
            // PAsaje de datos
            string funcion = datos.funcion;
            double Xini1 = datos.Xini1;
            double Xini2 = datos.Xini2;
            double tolerancia = datos.tolerancia;
            int iteraciones = datos.iteraciones;

            // Verifica si la sintaxis de la función es correcta
            if (!analizadorDeFunciones.Sintaxis(funcion, 'x'))
            {
                return BadRequest(new { mensaje = "Error al escribir la función" });
            }
            // Verifica que los valores iniciales no sean iguales
            if (Xini1 == Xini2)
            {
                return BadRequest(new { mensaje = "Los valores iniciales no pueden ser iguales" });
            }
            // Inicializa variables
            double xr = 0;
            double xrAnterior = Xini2;
            double error = double.MaxValue; // Inicializa el error con un valor muy grande
            int contador = 0;


            // Bucle principal del método de la secante
            while (error > tolerancia && contador < iteraciones)
            {
                contador++;

                xrAnterior = xr; // Guarda el valor anterior de xr

                var x1 = analizadorDeFunciones.EvaluaFx(Xini1);
                var x2 = analizadorDeFunciones.EvaluaFx(Xini2);

                // Calcula el nuevo valor de xr usando la fórmula de la secante
                xr = Xini2 - (x2 * (Xini1 - Xini2)) / (x1 - x2);  

                if (analizadorDeFunciones.EvaluaFx(xr) == 0)
                {
                    return Ok(new { mensaje = "xr es raíz", raiz = xr, iteraciones = contador });
                }

                // Si la función evaluada en xr es menor a la tolerancia, se considera que encontró la raíz
                if (Math.Abs(analizadorDeFunciones.EvaluaFx(xr)) < tolerancia)
                {
                    break; // Sal del while, raíz encontrada
                }

                // Calcula el error relativo solo si ya hay al menos dos iteraciones
                if (contador > 1)
                {
                    error = Math.Abs((xr - xrAnterior) / xr); // Error relativo entre iteraciones
                }

                Xini1 = Xini2;
                Xini2 = xr; // Actualiza los valores iniciales para la siguiente iteración
            }

            if (contador >= iteraciones)
            {
                return Ok(new {
                    mensaje = "No se encontró la raíz en el número de iteraciones especificado",
                    xr,                               
                    iteracionesRealizadas = contador,
                    errorFinal = error,
                    termino = "max_iter"
                });
            }
            else
            {
                return Ok(new {
                    mensaje = "Raíz encontrada",
                    xr,                                   
                    iteracionesRealizadas = contador,
                    errorFinal = error,
                    termino = "raiz"
                });
            }
        }
    }
}
