using Microsoft.Extensions.DependencyInjection;
using Editor.Services;

public static class ServiceRegistrator
{
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IProjectsService, ProjectsService>();
        }
}