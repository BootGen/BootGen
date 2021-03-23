using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Editor.Services;

namespace Editor.Controllers
{
    [ApiController]
    [Route("/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private IUsersService service;
        public UsersController(IUsersService service)
        {
            this.service = service;
        }
        
        [HttpGet]
        public IActionResult GetUsers()
        {
            var response = service.GetUsers();
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }

        [HttpGet]
        [Route("{userId}")]
        public IActionResult GetUser([FromRoute] int userId)
        {
            var response = service.GetUser(userId);
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }
    }
}
