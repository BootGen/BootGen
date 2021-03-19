using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using Editor.Services;

namespace Editor.Controllers
{
    [ApiController]
    [Route("registration")]
    public class RegistrationController : BaseController
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
    }
}
