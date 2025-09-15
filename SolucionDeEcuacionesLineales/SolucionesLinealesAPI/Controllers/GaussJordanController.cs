using MetodosDTO;
using Microsoft.AspNetCore.Mvc;

namespace SolucionesLinealesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GaussJordanController : ControllerBase
    {

        [HttpPost]
        public IActionResult Jordan([FromBody] GaussJordanDTO dto)
        {
            var matriz = dto.Matriz;

            for (int i = 0;i < matriz.Length; i++)
            {
                double coeficienteDiagonal = matriz[i][i];

                for(int j = 0;j < matriz[i].Length; j++)
                {
                    if(coeficienteDiagonal == 0)
                    {
                        return BadRequest("pivote nulo en fila " + i);
                    }
                    matriz[i][j] = matriz[i][j] / coeficienteDiagonal;
                }
                for(int k = 0; k < matriz.Length; k++)
                {
                    if (i != k)
                    {
                        double coeficienteCero = matriz[k][i];

                        for(int a = 0; a < matriz[i].Length; a++)
                        {
                            matriz[k][a] = matriz[k][a] - (coeficienteCero * matriz[i][a]);
                        }
                    }
                }
            }
            double[] vectorResultado= new double[matriz.Length];

            for (int i = 0; i < matriz.Length; i++)
            {
                bool sinSolucion = true;
                for(int j = 0; j < matriz[i].Length - 1; j++)
                {
                    if (matriz[i][j] != 0)
                    {
                        sinSolucion = false;
                        break;
                    }
                }


                if(sinSolucion && matriz[i][matriz[i].Length -1] != 0)
                {
                    return BadRequest("Sistema sin solucion");
                }

                vectorResultado[i] = matriz[i][matriz[i].Length - 1];

            }
            return Ok(vectorResultado);
           
        }
    }
}
