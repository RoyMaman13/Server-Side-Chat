using ChatWebServer.Models;
using ChatWebServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : Controller
    {
        private readonly Service _service;

        public RegisterController(Service service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("Id,Password,Nickname")] User user)
        {
            if (user == null || user.Id == "" || user.Id == null)
                return NotFound();
            User newUser = await _service.GetUser(user.Id);
            if (newUser != null)
                return NotFound();
            return (await _service.RegisterNewUser(user)) ? Ok(user) : NotFound();
        }

    }
}
