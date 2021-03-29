using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using Editor.Services;

namespace Editor.Controllers
{
    [ApiController]
    [Route("profile")]
    [Authorize]
    public class ProfileController : BaseController
    {
        private IProfileService service;
        public ProfileController(ApplicationDbContext dbContext, IProfileService service): base(dbContext)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Profile()
        {
            return Ok(CurrentUser);
        }

        [HttpPost]
        [Route("")]
        public IActionResult UpdateProfile([FromBody] User user)
        {
            service.CurrentUser = CurrentUser;
            return Ok(service.UpdateProfile(user));
        }

        [HttpPost]
        [Route("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordData data)
        {
            service.CurrentUser = CurrentUser;
            if (service.ChangePassword(data))
                return Ok();
            else
                return Unauthorized();
        }
    }
}
