using Microsoft.AspNetCore.Mvc;
using Editor.Services;
using Microsoft.AspNetCore.Authorization;

namespace Editor.Controllers
{
    [ApiController]
    [Route("/projects")]
    [Authorize]
    public class ProjectsController : BaseController
    {
        private IProjectsService service;
        public ProjectsController(ApplicationDbContext dbContext, IProjectsService service) : base(dbContext)
        {
            this.service = service;
        }
        
        [HttpGet]
        public IActionResult GetProjects()
        {
            service.CurrentUser = CurrentUser;
            return Ok(service.GetProjects());
        }

        [HttpGet]
        [Route("{projectId}")]
        public IActionResult GetProject([FromRoute] int projectId)
        {
            service.CurrentUser = CurrentUser;
            var project = service.GetProject(projectId);
            if ((project == null))
                return NotFound();
            return Ok(project);
        }

        [HttpPost]
        public IActionResult AddProject([FromBody] Project project)
        {
            service.CurrentUser = CurrentUser;
            return Ok(service.AddProject(project));
        }

        [HttpPut]
        [Route("{projectId}")]
        public IActionResult UpdateProject([FromRoute] int projectId, [FromBody] Project project)
        {
            service.CurrentUser = CurrentUser;
            var updatedProject = service.UpdateProject(projectId, project);
            if (updatedProject == null)
                return BadRequest();
            return Ok(updatedProject);
        }

        [HttpDelete]
        [Route("{projectId}")]
        public IActionResult DeleteProject([FromRoute] int projectId)
        {
            service.CurrentUser = CurrentUser;
            if (service.DeleteProject(projectId))
                return Ok();
            else
                return NotFound();
        }
    }
}
