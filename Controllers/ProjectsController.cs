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
    [Route("/projects")]
    public class ProjectsController : ControllerBase
    {
        private IProjectsService service;
        public ProjectsController(IProjectsService service)
        {
            this.service = service;
        }
        
        [HttpGet]
        public IActionResult GetProjects()
        {
            var response = service.GetProjects();
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
