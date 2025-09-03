using Microsoft.AspNetCore.Mvc;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("")]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Server is running");
        }
    }
}