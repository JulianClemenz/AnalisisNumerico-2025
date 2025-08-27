using Calculus;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Xml;

namespace ApiREST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewtonRaphsonController : ControllerBase
    {
        private Calculo analizadorDeFunciones = new Calculo();

        [HttpPost]
        public IActionResult PostMetodoAbierto([FromBody] DatosMetodoAbiertoDTO datos)
        {
            // Obtengo los datos del DTO enviado desde la petición HTTP
            string funcion = datos.funcion;       // Función matemática a evaluar
            double tolerancia = datos.tolerancia; // Tolerancia deseada para detener iteraciones
            int iteraciones = datos.iteraciones;  // Número máximo de iteraciones permitidas
            double Xini = datos.Xini;             // Valor inicial para Newton-Raphson

            // Verifica si la sintaxis de la función es correcta
            if (!analizadorDeFunciones.Sintaxis(funcion, 'x'))
            {
                return BadRequest(new { mensaje = "Error al escribir la función" });
            }

            int contador = 0;       // Contador de iteraciones realizadas
            double Xant = Xini;
            double error = double.MaxValue; // Inicializa el error con un valor muy grande
            double xr = Xini;    // Valor actual de la raíz (inicial = Xini)
            string xrResultado;
            string errorResultado;
            // Bucle principal del método Newton-Raphson
            while (error > tolerancia && contador < iteraciones)
            {
                
                Xant = xr;   // Guarda el valor anterior de xr                

                contador++; // Incrementa el número de iteraciones realizadas

                // Calcula la derivada numérica de la función en Xini
                double deri = (analizadorDeFunciones.EvaluaFx(Xini + tolerancia) - analizadorDeFunciones.EvaluaFx(Xini)) / tolerancia;

                // Si la derivada es muy pequeña, el método diverge
                if (Math.Abs(deri) < tolerancia)
                {
                    return BadRequest(new { mensaje = "EL METODO DIVERGE" });
                }

                // Calcula el nuevo valor de xr según Newton-Raphson: xr = Xini - f(Xini)/f'(Xini)
                xr = Xini - (analizadorDeFunciones.EvaluaFx(Xini) / deri);
                xrResultado = xr.ToString("F4");

                // Si la función evaluada en xr es menor a la tolerancia, se considera que encontró la raíz
                if (Math.Abs(analizadorDeFunciones.EvaluaFx(xr)) < tolerancia)
                {
                    break; // Sal del while, raíz encontrada
                }

                // Calcula el error relativo solo si ya hay al menos dos iteraciones
                if (contador > 1)
                {
                    error = Math.Abs((xr - Xant) / xr); // Error relativo entre iteraciones
                    errorResultado = error.ToString("F4");
                }

                Xini = xr;   // Actualiza Xini para la siguiente iteración

            }

            // Si se alcanzó el máximo de iteraciones sin cumplir tolerancia
            if (contador == iteraciones)
            {
                return Ok(new
                {
                    mensaje = "calculo finalizado, iteraciones maximas obtenidas",
                    xr,
                    errorFinal = error,
                    iteracionesRealizadas = contador,
                    termino = "max_iter"
                });
            }

            // Si se encontró la raíz antes de llegar al máximo de iteraciones
            return Ok(new
            {
                mensaje = "calculo finalizado, raiz encontrada en xr",
                xr,
                errorFinal = error,
                iteracionesRealizadas = contador,
                termino = "raiz"
            });
        }
    }
}
