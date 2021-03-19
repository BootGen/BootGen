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
    [Route("authentication")]
    public class AuthenticationController : BaseController
    {
        private IAuthenticationService service;
        public AuthenticationController(IAuthenticationService service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] AuthenticationData data)
        {
            var response = service.Login(data);
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }
    }
}
