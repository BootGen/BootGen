using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using WebProject.Services;

namespace WebProject.Controllers
{
    [ApiController]
    [Route("profile")]
    [Authorize]
    public class ProfileController : BaseController
    {
        private IProfileService service;
        public ProfileController(IProfileService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("profile")]
        public IActionResult Profile()
        {
            service.CurrentUser = CurrentUser;
            var response = service.Profile();
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }
        [HttpPost]
        [Route("update-profile")]
        public IActionResult UpdateProfile([FromBody] User user)
        {
            service.CurrentUser = CurrentUser;
            var response = service.UpdateProfile(user);
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }
        [HttpPost]
        [Route("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordData data)
        {
            service.CurrentUser = CurrentUser;
            var response = service.ChangePassword(data);
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }
    }
}
