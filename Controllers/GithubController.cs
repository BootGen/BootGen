using System;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Editor.Controllers
{
    [ApiController]
    [Route("github")]
    public class GithubController : ControllerBase
    {
        private ApplicationDbContext context;
        public GithubController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        [Route("hook")]
        public IActionResult Hook([FromBody] JsonElement content)
        {
            Console.WriteLine(content.GetType().FullName);
            context.GitHubEvents.Add(new GitHubEvent {
                TimeStamp = DateTime.Now,
                Content = content.ToString()
            });
            context.SaveChanges();
            return Ok();
        }
    }
}
