using MetodosDTO;
using Microsoft.AspNetCore.Mvc;

namespace SolucionesLinealesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GaussJordanController : ControllerBase
    {

        [HttpPost]
        public IActionResult Jordan([FromBody] GaussJordanDTO dto)//hacer llegar una lista o matriz q tenga los coeficiente de la diagonal principal
        {
            var matriz = dto.Matriz;

            for (int i = 0;i < matriz.Length; i++)
            {
                double coeficienteDiagonal = matriz[i][i];

                for(int j = 0;j < matriz[i].Length + 1; j++)
                {
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
                vectorResultado[i] = matriz[i][matriz[i].Length - 1];
            }

           
        }
    }
}
