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
            int grado = entrada.grado;

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
            if (grado < 1 || grado >= puntosCargados.Count)
            {
                return BadRequest("El grado debe ser al menos 1 y menor que la cantidad de puntos.");
            }
            double[][] matriz = GenerarMatrizPolinomial(grado, puntosCargados);

            //RESOLVER SISTEMA DE ECUACIONES, paso 2 POR GAUSS-JORDAN   
            for (int i = 0; i < matriz.Length; i++)
            {
                double coeficienteDiagonal = matriz[i][i];

                for (int j = 0; j < matriz[i].Length; j++)
                {
                    if (coeficienteDiagonal == 0)
                    {
                        return BadRequest("ERROR EN GAUSS-JORDAN: pivote nulo en fila " + i + 1);
                    }
                    matriz[i][j] = matriz[i][j] / coeficienteDiagonal;
                }
                for (int k = 0; k < matriz.Length; k++)
                {
                    if (i != k)
                    {
                        double coeficienteCero = matriz[k][i];

                        for (int a = 0; a < matriz[i].Length; a++)
                        {
                            matriz[k][a] = matriz[k][a] - (coeficienteCero * matriz[i][a]);
                        }
                    }
                }
            }
            double[] vectorResultado = new double[matriz.Length];

            for (int i = 0; i < matriz.Length; i++)
            {
                bool sinSolucion = true;
                for (int j = 0; j < matriz[i].Length - 1; j++)
                {
                    if (matriz[i][j] != 0)
                    {
                        sinSolucion = false;
                        break;
                    }
                }


                if (sinSolucion && matriz[i][matriz[i].Length - 1] != 0)
                {
                    return BadRequest("ERROR EN GAUSS-JORDAN: Sistema sin solucion");
                }

                vectorResultado[i] = matriz[i][matriz[i].Length - 1];

            }

            string funcion = string.Empty, signo = string.Empty;
            for(int i = 0; i < vectorResultado.Count(); i++)
            {
                double ai = Math.Round(vectorResultado[i], 4);
                if (i == 0 && ai != 0)
                {
                    funcion = $"{ai}";
                } else if(i==1 && ai != 0)
                {
                    funcion = $"{ai}x {signo}" + funcion;
                }
                else
                {
                    if (ai != 0)
                    {
                        funcion = $"{ai}x^{i} {signo}" + funcion;
                    }
                }
                signo = ai>0 ? "+" : string.Empty;
            }

            double x = 0, y = 0, sr = 0, st = 0, SumY = 0;
            int n = 0;
            foreach (double[] punto in puntosCargados)
            {
                x = punto[0];
                y = punto[1];
                double suma = 0;
                for (int i = 0; i < vectorResultado.Count(); i++)
                {
                    suma += vectorResultado[i] * Math.Pow(x, i);
                    SumY = punto[1];
                    n++;
                }
                sr += Math.Pow(suma - y, 2);  //cuantp varian los datos
                st += Math.Pow(SumY / n - y, 2);  //cuanto error tiene el programa
            }

            return Ok();
        }

        public double[][] GenerarMatrizPolinomial(int grado, List<double[]> puntosCargados)
        {
            int dimension = grado + 1;

            // Crear matriz dentada (array de arrays)
            double[][] matriz = new double[dimension][];
            for (int i = 0; i < dimension; i++)
            {
                matriz[i] = new double[dimension + 1];
            }

            // Variables temporales
            double x = 0;
            double y = 0;

            // Calcular sumatorias para las ecuaciones normales
            foreach (double[] punto in puntosCargados)
            {
                x = punto[0];
                y = punto[1];

                for (int fila = 0; fila < dimension; fila++)
                {
                    for (int col = 0; col < dimension; col++)
                    {
                        matriz[fila][col] += Math.Pow(x, fila + col);
                    }

                    matriz[fila][dimension] += Math.Pow(x, fila) * y;
                }
            }

            return matriz;
        }

    }
}