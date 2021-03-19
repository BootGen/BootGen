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
    public class ProjectsService : IProjectsService
    {

        public ServiceResponse<List<Project>> GetProjects()
        {
            using (var db = new DataContext())
            {
                var query = db.Projects;
                return new ServiceResponse<List<Project>>
                {
                    StatusCode = 200,
                    ResponseData = query.ToList()
                };
            }
        }

        public ServiceResponse<List<Project>> GetProjectsOfOwner(int userId)
        {
            using (var db = new DataContext())
            {
                var query = db.Projects;
                return new ServiceResponse<List<Project>>
                {
                    StatusCode = 200,
                    ResponseData = query.ToList()
                };
            }
        }

        public ServiceResponse<Project> GetProject(int projectId)
        {
            using (var db = new DataContext())
            {
                var item = db.Projects
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
        }

        public ServiceResponse<Project> AddProject(Project project)
        {
            EntityEntry<Project> entityEntry;
            try
            {
                using (var db = new DataContext())
                {
                    entityEntry = db.Projects.Add(project);
                    db.SaveChanges();
                }
            } catch (DbUpdateException)
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
                using (var db = new DataContext())
                {
                    var original = db.Projects
                                .Where(item => item.Id == projectId).FirstOrDefault();
                    EntityEntry<Project> entityEntry;
                    entityEntry = db.Projects.Update(original);
                    original.Name = project.Name;
                    original.Json = project.Json;
                    original.OwnerId = project.OwnerId;
                    db.SaveChanges();
                    return new ServiceResponse<Project>
                    {
                        StatusCode = 200,
                        ResponseData = entityEntry.Entity
                    };
                }
            } catch (DbUpdateException)
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
                using (var db = new DataContext())
                {
                    var item = db.Projects.Where(item => item.Id == projectId).FirstOrDefault();
                    if (item == null)
                        return new ServiceResponse
                        {
                            StatusCode = 400
                        };
                    db.Projects.Remove(item);
                    db.SaveChanges();
                }
            } catch (DbUpdateException)
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
