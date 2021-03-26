using Microsoft.AspNetCore.Mvc;
using Editor.Services;

namespace Editor.Controllers
{
    [ApiController]
    [Route("registration")]
    public class RegistrationController : ControllerBase
    {
        private IRegistrationService service;
        public RegistrationController(IRegistrationService service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] RegistrationData data)
        {
            var response = service.Register(data);
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }

        [HttpPost]
        [Route("activate")]
        public IActionResult Activate(string activationCode)
        {
            if (service.Activate(activationCode))
                return Ok();
            return BadRequest();
        }
    }
}
