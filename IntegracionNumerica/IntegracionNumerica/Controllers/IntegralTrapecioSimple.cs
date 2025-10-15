using Calculus;
using Entity;
using Microsoft.AspNetCore.Mvc;

namespace IntegracionNumerica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetodosIntegrales : ControllerBase
    {
        //ENDPOINTS
        [HttpPost("trapecio/simple")]
        public ActionResult<DatosSalida> TrapecioSimple([FromBody] DatosEntrada datos)
            => Ejecutar(() => CalcularIntegralTrapeciosSimple(datos));

        [HttpPost("trapecio/multiple")]
        public ActionResult<DatosSalida> TrapecioMultiple([FromBody] DatosEntrada datos)
            => Ejecutar(() => CalcularIntegralTrapeciosMultiple(datos));

        [HttpPost("simpson/1-3/simple")]
        public ActionResult<DatosSalida> IntegralSimpsonSimple1_3([FromBody] DatosEntrada datos)
            => Ejecutar(() => CalcularIntegralSimpsonUnTercioSimple(datos));

        [HttpPost("simpson/1-3/multiple")]
        public ActionResult<DatosSalida> IntegralSimpsonMultiple1_3([FromBody] DatosEntrada datos)
            => Ejecutar(() => CalcularIntegralSimpsonUnTercioMultiple(datos));

        [HttpPost("simpson/3_8")]
        public ActionResult<DatosSalida> IntegralSimpson3_8([FromBody] DatosEntrada datos)
            => Ejecutar(() => CalcularIntegralSimpsonTresOctavos(datos));

        [HttpPost("combinados")]
        public ActionResult<DatosSalida> IntegralCombinados([FromBody] DatosEntrada datos)
            => Ejecutar(() => CalcularCombinados(datos));

        //METODO GENERAL PARA EJECUTAR
        private ActionResult<DatosSalida> Ejecutar(Func<double> calcular)
        {
            try
            {
                var area = calcular();
                return Ok(new DatosSalida { area = area });
            }
            catch (ArgumentException ex)
            {
                // Errores de validación (función inválida, n inválido, etc.)
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Otros errores inesperados
                return StatusCode(500, new { error = "Error interno", detail = ex.Message });
            }
        }


        // TRAPECIO SIMPLE
        [NonAction]
        public double CalcularIntegralTrapeciosSimple(DatosEntrada datos)
        {
            var Funcion = new Calculo();
            string funcion = datos.funcion;
            double a = datos.Xi;
            double b = datos.Xd;

            if (!Funcion.Sintaxis(funcion, 'x'))
                throw new ArgumentException("Función inválida.");

            // (f(a)+f(b))*(b-a)/2
            return ((Funcion.EvaluaFx(a) + Funcion.EvaluaFx(b)) * (b - a)) / 2.0;
        }

        // TRAPECIO MÚLTIPLE
        [NonAction]
        public double CalcularIntegralTrapeciosMultiple(DatosEntrada datos)
        {
            var Funcion = new Calculo();
            string funcion = datos.funcion;
            double a = datos.Xi;
            double b = datos.Xd;
            int n = datos.CantSubIntervalos;

            if (!Funcion.Sintaxis(funcion, 'x'))
                throw new ArgumentException("Función inválida.");
            if (n <= 0)
                throw new ArgumentException("n debe ser positivo.");

            double h = (b - a) / n;
            double sum = 0.0;

            // suma de i=1..n-1
            for (int i = 1; i <= n - 1; i++)
                sum += Funcion.EvaluaFx(a + h * i);

            return (h / 2.0) * (Funcion.EvaluaFx(a) + 2.0 * sum + Funcion.EvaluaFx(b));
        }

        // SIMPSON 1/3 SIMPLE
        [NonAction]
        public double CalcularIntegralSimpsonUnTercioSimple(DatosEntrada datos)
        {
            var Funcion = new Calculo();
            string funcion = datos.funcion;
            double a = datos.Xi;
            double b = datos.Xd;

            if (!Funcion.Sintaxis(funcion, 'x'))
                throw new ArgumentException("Función inválida.");

            // h = (b-a)/2, f(a) + 4*f(a+h) + f(b)
            double h = (b - a) / 2.0;
            double mid = a + h;

            return (h / 3.0) * (Funcion.EvaluaFx(a) + 4.0 * Funcion.EvaluaFx(mid) + Funcion.EvaluaFx(b));
        }

        // SIMPSON 1/3 MÚLTIPLE (n par)
        [NonAction]
        public double CalcularIntegralSimpsonUnTercioMultiple(DatosEntrada datos)
        {
            var Funcion = new Calculo();
            string funcion = datos.funcion;
            double a = datos.Xi;
            double b = datos.Xd;
            int n = datos.CantSubIntervalos;

            if (!Funcion.Sintaxis(funcion, 'x'))
                throw new ArgumentException("Función inválida.");
            if (n <= 0 || (n % 2) != 0)
                throw new ArgumentException("Para Simpson 1/3 múltiple, n debe ser par y > 0.");

            double h = (b - a) / n;
            double sumImpares = 0.0; // i = 1,3,5,...
            double sumPares = 0.0;   // i = 2,4,6,...

            for (int i = 1; i <= n - 1; i++)
            {
                double x = a + h * i;
                if ((i % 2) == 0) sumPares += Funcion.EvaluaFx(x);
                else sumImpares += Funcion.EvaluaFx(x);
            }

            return (h / 3.0) * (Funcion.EvaluaFx(a) + 4.0 * sumImpares + 2.0 * sumPares + Funcion.EvaluaFx(b));
        }

        // SIMPSON 3/8 (tres subintervalos)
        [NonAction]
        public double CalcularIntegralSimpsonTresOctavos(DatosEntrada datos)
        {
            var Funcion = new Calculo();
            string funcion = datos.funcion;
            double a = datos.Xi;
            double b = datos.Xd;

            if (!Funcion.Sintaxis(funcion, 'x'))
                throw new ArgumentException("Función inválida.");

            // h = (b-a)/3, f(a) + 3*f(a+h) + 3*f(a+2h) + f(b)
            double h = (b - a) / 3.0;

            return (3.0 * h / 8.0) * (
                Funcion.EvaluaFx(a)
                + 3.0 * Funcion.EvaluaFx(a + h)
                + 3.0 * Funcion.EvaluaFx(a + 2.0 * h)
                + Funcion.EvaluaFx(b)
            );
        }

        // COMBINADO: 3/8 (si n impar) + 1/3 múltiple
        [NonAction]
        public double CalcularCombinados(DatosEntrada datos)
        {
            var Funcion = new Calculo();
            string funcion = datos.funcion;
            double a = datos.Xi;
            double b = datos.Xd;
            int n = datos.CantSubIntervalos;

            if (string.IsNullOrWhiteSpace(funcion))
                throw new ArgumentException("Función mal ingresada.");
            if (!Funcion.Sintaxis(funcion, 'x'))
                throw new ArgumentException("Función con sintaxis inválida.");
            if (n <= 0)
                throw new ArgumentException("n debe ser positivo.");

            double h = (b - a) / n;
            double resultado = 0.0;

            // Si n es impar, integro los últimos 3 subintervalos con 3/8 y ajusto
            if ((n % 2) != 0)
            {
                if (n < 3)
                    throw new ArgumentException("Para n impar se requieren al menos 3 subintervalos (3/8).");

                double nuevoXi = a + h * (n - 3);

                resultado += CalcularIntegralSimpsonTresOctavos(new DatosEntrada
                {
                    funcion = funcion,
                    Xi = nuevoXi,
                    Xd = b,
                    CantSubIntervalos = 3
                });

                // Ajusto el tramo restante para 1/3 múltiple
                n -= 3;      // ahora n es par
                b = nuevoXi; // evito recalcular lo ya integrado
                h = (b - a) / n;
            }

            // Simpson 1/3 múltiple en el tramo restante [a,b] con n par
            double sumImpares = 0.0, sumPares = 0.0;
            for (int i = 1; i <= n - 1; i++)
            {
                double x = a + h * i;
                if ((i % 2) == 0) sumPares += Funcion.EvaluaFx(x);
                else sumImpares += Funcion.EvaluaFx(x);
            }

            double s13 = (h / 3.0) * (Funcion.EvaluaFx(a) + 4.0 * sumImpares + 2.0 * sumPares + Funcion.EvaluaFx(b));
            resultado += s13;

            return resultado;
        }
    }
}



