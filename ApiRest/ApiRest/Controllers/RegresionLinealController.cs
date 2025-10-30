using Entity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;
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
            if (entrada == null || entrada.puntosCargados == null)
                return BadRequest("Entrada inválida:Campos nulos");

            double tolerancia = entrada.Tolerancia;
            List<double[]> puntosCargados = entrada.puntosCargados;

            double SumaX = 0, SumaY = 0, SumaXY = 0, SumaX2 = 0;
            double a1 = 0, a0 = 0, St = 0, Sr = 0;
            int n = puntosCargados.Count;

            if (n < 2) return BadRequest("Se requieren al menos 2 puntos.");

            foreach (var p in puntosCargados)
                if (p == null || p.Length < 2)
                    return BadRequest("Cada punto debe tener al menos [x,y].");

            // Sumatorias
            for (int i = 0; i < n; ++i)
            {
                SumaX += puntosCargados[i][0];
                SumaY += puntosCargados[i][1];
                SumaXY += puntosCargados[i][0] * puntosCargados[i][1];
                SumaX2 += puntosCargados[i][0] * puntosCargados[i][0];
            }

            if (n * SumaX2 - Math.Pow(SumaX, 2) == 0)
                return BadRequest("No se puede calcular la regresión con dicha lista de puntos: división por cero");

            // Coeficientes
            a1 = (n * SumaXY - SumaX * SumaY) / (n * SumaX2 - Math.Pow(SumaX, 2));
            a0 = (SumaY / n) - a1 * (SumaX / n);

            // St y Sr
            for (int j = 0; j < n; ++j)
            {
                St += Math.Pow((SumaY / n - puntosCargados[j][1]), 2);
                Sr += Math.Pow((a1 * puntosCargados[j][0] + a0 - puntosCargados[j][1]), 2);
            }

            if (St == 0)
            {
                var salidafallida = new DatoSalidaGeneral
                {
                    Funcion = $"y = {a1}x {(a0 >= 0 ? "+" : "-")} {Math.Abs(a0)}",
                    PorcCorrelacion = 0,
                    EfectAjuste = 0 > tolerancia,

                    FuncionModificada = $"y = {a1:F2}x {(a0 >= 0 ? "+" : "-")} {Math.Abs(a0):F2}",
                    PorcCorrelacionModificada = 0,
                    EfectAjusteModificada = 0 > tolerancia
                };
                return Ok(salidafallida);
            }

            // r normal
            double ratioN = (St - Sr) / St;
            if (ratioN < 0) ratioN = 0;
            double r = Math.Sqrt(ratioN) * 100;

            // Recta modificada (redondeada) y r modificado
            string funcionMod = $"y = {a1:F2}x {(a0 >= 0 ? "+" : "-")} {Math.Abs(a0):F2}";
            double rMod = CalcularCoeficienteCorrelacion(puntosCargados, funcionMod);

            var salida = new DatoSalidaGeneral
            {
                Funcion = $"y = {a1}x {(a0 >= 0 ? "+" : "-")} {Math.Abs(a0)}",
                PorcCorrelacion = r,
                EfectAjuste = r > tolerancia,

                FuncionModificada = funcionMod,
                PorcCorrelacionModificada = rMod,
                EfectAjusteModificada = rMod > tolerancia
            };

            return Ok(salida);
        }

        // ===== NUEVO: recalcular r con recta ingresada por el usuario =====
        public class RecalcEntrada
        {
            public List<double[]> puntosCargados { get; set; }
            public string Funcion { get; set; }    // ej: "y = 1x - 3"
            public double Tolerancia { get; set; } // opcional
        }
        public class RecalcSalida
        {
            public string Funcion { get; set; }
            public double PorcCorrelacion { get; set; }
            public bool EfectAjuste { get; set; }
        }

        [HttpPost("RecalcularR")]
        public IActionResult RecalcularR([FromBody] RecalcEntrada entrada)
        {
            if (entrada == null || entrada.puntosCargados == null || string.IsNullOrWhiteSpace(entrada.Funcion))
                return BadRequest("Datos incompletos: se requiere lista de puntos y una función.");

            if (entrada.puntosCargados.Count < 2)
                return BadRequest("Se requieren al menos 2 puntos.");

            foreach (var p in entrada.puntosCargados)
                if (p == null || p.Length < 2)
                    return BadRequest("Cada punto debe tener al menos [x,y].");

            try
            {
                // normalizo por si vino con “+ -” o menos unicode
                var f = NormalizarFuncion(entrada.Funcion);
                double r = CalcularCoeficienteCorrelacion(entrada.puntosCargados, f);

                return Ok(new RecalcSalida
                {
                    Funcion = f,
                    PorcCorrelacion = r,
                    EfectAjuste = r > entrada.Tolerancia
                });
            }
            catch (FormatException ex)
            {
                return BadRequest($"Función inválida: {ex.Message}");
            }
        }

        // ================== Helpers ==================
        [NonAction]
        private static string NormalizarFuncion(string funcion)
        {
            if (funcion == null) return null;
            return funcion
                .Replace("−", "-")     // menos unicode
                .Replace("+ -", "- ")  // "+ -" → "- "
                .Replace("- -", "+ ")  // "- -" → "+ "
                .Trim();
        }

        [NonAction]
        private Tuple<double, double> ObtenerCoeficientesFuncion(string funcion)
        {
            if (string.IsNullOrWhiteSpace(funcion))
                throw new ArgumentException("La funcion no puede estar vacia");

            funcion = NormalizarFuncion(funcion);

            // y = a1 x +/- a0   (con coma o punto decimal)
            var regex = new Regex(
                @"y\s*=\s*([+-]?\d+(?:[.,]\d+)?)\s*x\s*([+-]\s*\d+(?:[.,]\d+)?)",
                RegexOptions.IgnoreCase
            );
            var match = regex.Match(funcion);

            if (!match.Success)
                throw new FormatException("Formato invalido.Ejemplo esperado: y = 2.5x - 1.3");

            string a1Str = match.Groups[1].Value.Replace(',', '.').Trim();
            string a0Str = match.Groups[2].Value.Replace(',', '.').Replace(" ", "");

            double a1 = double.Parse(a1Str, CultureInfo.InvariantCulture);
            double a0 = double.Parse(a0Str, CultureInfo.InvariantCulture);

            return Tuple.Create(a1, a0);
        }

        [NonAction]
        private double CalcularCoeficienteCorrelacion(List<double[]> puntosCargados, string funcion)
        {
            funcion = NormalizarFuncion(funcion);

            double sumY = 0;
            foreach (var punto in puntosCargados) sumY += punto[1];

            var (a1, a0) = ObtenerCoeficientesFuncion(funcion);

            double st = 0, sr = 0;
            double yProm = sumY / puntosCargados.Count;

            foreach (var punto in puntosCargados)
            {
                st += Math.Pow(yProm - punto[1], 2);
                sr += Math.Pow(a1 * punto[0] + a0 - punto[1], 2);
            }

            if (st == 0) return 0; // todas las Y iguales → r indefinido

            double ratio = (st - sr) / st;
            if (ratio < 0) ratio = 0; // por redondeos numéricos
            return Math.Sqrt(ratio) * 100;
        }
    }
}
