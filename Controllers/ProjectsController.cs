using Microsoft.AspNetCore.Mvc;
using Editor.Services;

namespace Editor.Controllers
{
    [ApiController]
    [Route("/projects")]
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
            var response = service.GetProjectsOfOwner(CurrentUser.Id);
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }

        [HttpGet]
        [Route("{projectId}")]
        public IActionResult GetProject([FromRoute] int projectId)
        {
            var response = service.GetProject(projectId);
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }

        [HttpPost]
        public IActionResult AddProject([FromBody] Project project)
        {
            var response = service.AddProject(project);
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }

        [HttpPut]
        [Route("{projectId}")]
        public IActionResult UpdateProject([FromRoute] int projectId, [FromBody] Project project)
        {
            var response = service.UpdateProject(projectId, project);
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }

        [HttpDelete]
        [Route("{projectId}")]
        public IActionResult DeleteProject([FromRoute] int projectId)
        {
            var response = service.DeleteProject(projectId);
            return new ObjectResult(null) { StatusCode = response.StatusCode };
        }
    }
}
