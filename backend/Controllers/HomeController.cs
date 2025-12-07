using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return Content("API do Sistema de Reservas GS está online 🚀");
        }
    }
}
