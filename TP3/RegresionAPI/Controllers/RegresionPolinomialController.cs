using Entity;
using Microsoft.AspNetCore.Mvc;
using TP3;

namespace RegresionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegresionPolinomialController : ControllerBase
    {
        [HttpPost]
        public IActionResult RegresionPolinomial(DatoEntradaRPDto entrada)
        {

            if (entrada == null || entrada.puntosCargados == null)//verificamos si los campos de entrada no son nulos
            {
                return BadRequest("Entrada inválida:Campos nulos");
            }

            double tolerancia = entrada.Tolerancia;
            List<Double[]> puntosCargados = entrada.puntosCargados;
            double grado = entrada.grado;

            //SETEAR VARIABLES

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

            //paso 1
            double[,] matriz = GenerarMatrizPolinomial(grado, puntosCargados);

            //RESOLVER SISTEMA DE ECUACIONES, paso 2
            matriz = GaussJordan(matriz);




        }
    }


    public double[,] GenerarMatrizPolinomial(int grado, List<double[]> puntosCargados) {
            int dimension = grado + 1;
            double[,] matriz = new double[dimension, dimension + 1];
            double x = 0; double y = 0;
            foreach (double[] punto in puntosCargados)
            {
                x = punto[0];
                y = punto[1];
                for (int fila = 0; fila < dimension; fila++)
                {
                    for (int col = 0; col < dimension; col++)
                    {
                        matriz[fila, col] += Math.Pow(x, fila + col);
                    }
                    matriz[fila, dimension] += Math.Pow(x, fila) * y;
                }
            }
            return matriz;
        }

        public static double[,] GaussJordan(double[,] matriz)
        {
            int filas = matriz.GetLength(0);
            int cols = matriz.GetLength(1);

            for (int i = 0; i < filas; i++)
            {
                double coeficienteDiagonal = matriz[i, i];

                for (int j = 0; j < cols; j++)
                {
                    if (coeficienteDiagonal == 0)
                    {
                        throw new InvalidOperationException("pivote nulo en fila " + (i + 1));
                    }
                    matriz[i, j] = matriz[i, j] / coeficienteDiagonal;
                }

                for (int k = 0; k < filas; k++)
                {
                    if (i != k)
                    {
                        double coeficienteCero = matriz[k, i];

                        for (int a = 0; a < cols; a++)
                        {
                            matriz[k, a] = matriz[k, a] - (coeficienteCero * matriz[i, a]);
                        }
                    }
                }
            }

            // Chequeo de inconsistencia (fila 0...0 | b ≠ 0)
            for (int i = 0; i < filas; i++)
            {
                bool sinSolucion = true;

                for (int j = 0; j < cols - 1; j++)
                {
                    if (matriz[i, j] != 0)
                    {
                        sinSolucion = false;
                        break;
                    }
                }

                if (sinSolucion && matriz[i, cols - 1] != 0)
                {
                    throw new InvalidOperationException("Sistema sin solucion");
                }
            }

            return matriz;
        }
    }
}