using MetodosDTO;
using Microsoft.AspNetCore.Mvc;

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

        }
    }
}
