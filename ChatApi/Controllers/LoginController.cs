using ChatWebServer.Models;
using ChatWebServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly Service _service;

        public LoginController(Service service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("Id,Password")] User user)
        {
            User claim = await _service.UserValidation(user);
            if (claim == null)
                return NotFound();
            return Ok(claim);
        }
    }
}
