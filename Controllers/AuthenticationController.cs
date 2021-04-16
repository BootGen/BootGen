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
    public class AuthenticationController : ControllerBase
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
            if (response.Success) {
                return Ok(new LoginSuccess {
                    Jwt = response.Jwt
                });
            }

            var error = new LoginError
            {
                IsInactive = response.IsInactive,
                WrongCredentials = response.WrongCredentials
            };
            return new ObjectResult(error) { StatusCode = 401 };
        }
    }
}
