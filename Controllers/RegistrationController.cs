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
            return Ok(service.Register(data));
        }

        [HttpPost]
        [Route("activate")]
        public IActionResult Activate(string activationToken)
        {
            if (service.Activate(activationToken))
                return Ok();
            return BadRequest();
        }
    }
}
