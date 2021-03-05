using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebProject.Services
{
    public interface IProjectsService
    {
        ServiceResponse<List<Project>> GetProjects();
        ServiceResponse<Project> GetProject(int projectId);
        ServiceResponse<List<Project>> GetProjectsOfOwner(int userId);
        ServiceResponse<Project> AddProject(Project project);
        ServiceResponse<Project> UpdateProject(int projectId, Project project);
        ServiceResponse DeleteProject(int projectId);
    }
}
