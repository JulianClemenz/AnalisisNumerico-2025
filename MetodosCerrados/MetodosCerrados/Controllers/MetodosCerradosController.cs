using Microsoft.AspNetCore.Mvc;
//using ProgramaUnoAnalisisNumerico;
using Calculus;
using DTO;
namespace CalculoREST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetodosCerradosController : ControllerBase
    {
        private Calculo analizadorDeFunciones = new Calculo();


        [HttpPost]
        public IActionResult calcular([FromBody] DatosMetodoCerradoDTO datos)
        {
            string funcion = datos.funcion;
            double Xi = datos.Xi;
            double Xd = datos.Xd;
            int iteraciones = datos.iteraciones;
            double tolerancia = datos.tolerancia;

            if (!analizadorDeFunciones.Sintaxis(funcion, 'x'))
            {
                return BadRequest(new { mensaje = "Error al escribir la función" });
            }

            if (analizadorDeFunciones.EvaluaFx(Xi) * analizadorDeFunciones.EvaluaFx(Xd) > 0) //esta operacion verifica que haya una raiz entre ambas funciones tienen signos opuestos
            {
                return BadRequest(new { mensaje = "Xi y Xd no encierran una raíz" });
            }
            if (analizadorDeFunciones.EvaluaFx(Xi) * analizadorDeFunciones.EvaluaFx(Xd) == 0)
            {
                if (analizadorDeFunciones.EvaluaFx(Xi) == 0)
                {
                    return Ok(new {
                        mensaje = "Xi es raíz",
                        xr = Xi,
                        errorFinal = 0d,
                        iteracionesRealizadas = 0,
                        termino = "raiz"
                    });
                }
                else
                {
                    return Ok(new {
                        mensaje = "Xd es raíz",
                        xr = Xd,
                        errorFinal = 0d,
                        iteracionesRealizadas = 0,
                        termino = "raiz"
                    });
                }
            }
            else
            {
                double xr = 0;
                double xrAnterior = 0;
                double error = 1;
                int contador = 0;

                while (error > tolerancia && contador < iteraciones)
                {
                    if (datos.metodo) // true para regla falsa, false para bisección
                    {
                        xr = ((analizadorDeFunciones.EvaluaFx(Xd) * Xi) - (analizadorDeFunciones.EvaluaFx(Xi) * Xd)) / (analizadorDeFunciones.EvaluaFx(Xd) - analizadorDeFunciones.EvaluaFx(Xi));
                    }
                    else
                    {
                        xr = (Xi + Xd) / 2;
                    }

                    double fxr = analizadorDeFunciones.EvaluaFx(xr);

                    error = Math.Abs((Xd - Xi) / xr);

                    if (error < tolerancia || Math.Abs(fxr) < tolerancia)
                    {
                        return Ok(new {
                            mensaje = $"Raiz encontrada",
                            xr,
                            errorFinal = error,
                            iteracionesRealizadas = contador,
                            termino = "raiz"
                        });
                    }
                    else
                    {
                        if (analizadorDeFunciones.EvaluaFx(Xi) * fxr > 0)
                        {
                            Xi = xr;
                        }
                        else
                        {
                            Xd = xr;
                        }

                        xrAnterior = xr;
                    }
                    contador++;
                }

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
    }
