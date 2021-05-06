using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Editor.Services
{
    public class ProjectsService : IProjectsService
    {
        public User CurrentUser { get; set; }
        private readonly ApplicationDbContext dbContext;

        public ProjectsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public List<Project> GetProjects()
        {
            return dbContext.Projects.Where(p => p.OwnerId == CurrentUser.Id).ToList();
        }

        public Project GetProject(int projectId)
        {
            return dbContext.Projects
                         .Where(p => p.Id == projectId && p.OwnerId == CurrentUser.Id)
                         .FirstOrDefault();
        }

        public Project AddProject(Project project)
        {
            try
            {
                var entityEntry = dbContext.Projects.Add(project);
                dbContext.SaveChanges();
                return entityEntry.Entity;
            }
            catch (DbUpdateException)
            {
                return null;
            }
        }

        public Project UpdateProject(int projectId, Project project)
        {
            try
            {
                var original = GetProject(projectId);
                if (original == null)
                    return null;
                var entityEntry = dbContext.Projects.Update(original);
                original.Name = project.Name;
                original.Json = project.Json;
                original.OwnerId = project.OwnerId;
                original.Backend = project.Backend;
                original.Frontend = project.Frontend;
                dbContext.SaveChanges();
                return entityEntry.Entity;
            }
            catch (DbUpdateException)
            {
                return null;
            }
        }

        public bool DeleteProject(int projectId)
        {
            try
            {
                var item = dbContext.Projects.Where(item => item.Id == projectId).FirstOrDefault();
                if (item == null)
                    return false;
                dbContext.Projects.Remove(item);
                dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}
