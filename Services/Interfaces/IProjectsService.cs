using System.Collections.Generic;

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
