using System;
using System.Net.Http.Headers;
using System.Text;
using Editor.Config;
using Editor.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Editor.Services;
using Editor.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Editor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                                builder =>
                                {
                                    builder.WithOrigins(Configuration["ClientUrl"])
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                                });
            });

            services.AddControllers();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddHttpClient("GithubApiClient",
                                   client =>
                                   {
                                       client.BaseAddress = new Uri("https://api.github.com/");
                                       client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("BootGen", "1.0"));
                                       client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                                   });

            services.AddHttpClient("GithubClient",
                                   client =>
                                   {
                                       client.BaseAddress = new Uri("https://github.com/");
                                       client.DefaultRequestHeaders
                                             .Accept
                                             .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                   });

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IGenerateService, GenerateService>();
            services.AddScoped<IProjectsService, ProjectsService>();
            services.AddScoped<IErrorService, ErrorService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IGithubService, GithubService>();
            services.AddScoped<IOAuthService, OAuthService>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySQL"), ServerVersion.FromString(Configuration["DataBaseVersion"])));
            services.AddSingleton(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorLoggingMiddleware>();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            if (env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                });
            }
        }
    }
}
