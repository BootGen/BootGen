using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Editor.Services
{
    public interface IProjectsService
    {
        User CurrentUser { get; set; }
        List<Project> GetProjects();
        Project GetProject(int projectId);
        Project AddProject(Project project);
        Project UpdateProject(int projectId, Project project);
        bool DeleteProject(int projectId);
    }
}
