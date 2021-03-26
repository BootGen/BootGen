using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Editor.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly ApplicationDbContext dbContext;

        public ProjectsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public ServiceResponse<List<Project>> GetProjects()
        {
            var query = dbContext.Projects;
            return new ServiceResponse<List<Project>>
            {
                StatusCode = 200,
                ResponseData = query.ToList()
            };
        }

        public ServiceResponse<List<Project>> GetProjectsOfOwner(int userId)
        {
            var query = dbContext.Projects;
            return new ServiceResponse<List<Project>>
            {
                StatusCode = 200,
                ResponseData = query.ToList()
            };
        }

        public ServiceResponse<Project> GetProject(int projectId)
        {
            var item = dbContext.Projects
                         .Where(item => item.Id == projectId).FirstOrDefault();
            if (item == null)
                return new ServiceResponse<Project>
                {
                    StatusCode = 404
                };
            return new ServiceResponse<Project>
            {
                StatusCode = 200,
                ResponseData = item
            };
        }

        public ServiceResponse<Project> AddProject(Project project)
        {
            EntityEntry<Project> entityEntry;
            try
            {
                entityEntry = dbContext.Projects.Add(project);
                dbContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return new ServiceResponse<Project>
                {
                    StatusCode = 400
                };
            }
            return new ServiceResponse<Project>
            {
                StatusCode = 200,
                ResponseData = entityEntry.Entity
            };
        }

        public ServiceResponse<Project> UpdateProject(int projectId, Project project)
        {
            try
            {
                var original = dbContext.Projects
                            .Where(item => item.Id == projectId).FirstOrDefault();
                var entityEntry = dbContext.Projects.Update(original);
                original.Name = project.Name;
                original.Json = project.Json;
                original.OwnerId = project.OwnerId;
                dbContext.SaveChanges();
                return new ServiceResponse<Project>
                {
                    StatusCode = 200,
                    ResponseData = entityEntry.Entity
                };
            }
            catch (DbUpdateException)
            {
                return new ServiceResponse<Project>
                {
                    StatusCode = 400
                };
            }
        }

        public ServiceResponse DeleteProject(int projectId)
        {
            try
            {
                var item = dbContext.Projects.Where(item => item.Id == projectId).FirstOrDefault();
                if (item == null)
                    return new ServiceResponse
                    {
                        StatusCode = 400
                    };
                dbContext.Projects.Remove(item);
                dbContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return new ServiceResponse
                {
                    StatusCode = 400
                };
            }
            return new ServiceResponse
            {
                StatusCode = 200
            };
        }
    }
}
