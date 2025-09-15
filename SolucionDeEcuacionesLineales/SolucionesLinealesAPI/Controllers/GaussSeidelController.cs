using MetodosDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;

namespace SolucionesLinealesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GaussSeidelController : ControllerBase
    {

        [HttpPost]
        public IActionResult Seidel([FromBody] GaussSeidelDTO dto)
        {
            var matriz = dto.Matriz;

            bool esDD = true;
            for (int i = 0; i < matriz.Length; i++)//metodo para verificar si es D.D
            {
                double diag = Math.Abs(matriz[i][i]);
                double suma = 0;
                for (int j = 0; j < matriz.Length; j++)
                    if (j != i) suma += Math.Abs(matriz[i][j]);  

                if (diag <= suma) { esDD = false; break; }       
            }

            if (!esDD)
            {
                return BadRequest("La matriz no es diagonalmente dominante. El método Gauss-Seidel puede no converger.");
            }

            double tolerancia = 0.0001;
            bool esSolucion = false;
            int contador = 0;
            double[] vectorResultado = new double[matriz.Length];
            vectorResultado.Initialize();
            double[] vectorAnterior = new double[matriz.Length];

            


            while (contador <= 100 && !esSolucion)
            {
                contador++;
                if(contador > 1)
                {
                    vectorResultado.CopyTo(vectorAnterior, 0);
                }
                for(int i = 0; i < matriz.Length; i++)
                {
                    double resultado = matriz[i][ matriz[i].Length - 1];
                    double coeficienteIncognita = matriz[i][i];
                    for(int j = 0; j < matriz.Length; j++)
                    {
                        if(i != j)
                        {
                            resultado = resultado - (matriz[i][j] * vectorResultado[j]);
                        }
                    }
                    coeficienteIncognita = resultado / coeficienteIncognita;
                    vectorResultado[i] = coeficienteIncognita;
                }
                int contadorMismoResultado = 0;
                double errorRelativo = 0;

                for(int i = 0; i < matriz.Length; i++)
                {
                    errorRelativo = Math.Abs((vectorResultado[i] - vectorAnterior[i]) / vectorResultado[i]);
                    if(errorRelativo < tolerancia)
                    {
                        contadorMismoResultado++;
                    }
                }
                esSolucion = contadorMismoResultado == matriz.Length;
            }

            if(contador <= 100)
            {
                return Ok(vectorResultado);
            } else
            {
                return BadRequest("se ha alcanzado al maximo de iteraiciones");
            }



        }
    }
}
