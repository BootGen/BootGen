using System.Linq;
using Editor.Services.Interfaces;

namespace Editor.Services
{
    public class OAuthService : IOAuthService
    {
        private ApplicationDbContext dbContext;

        public OAuthService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public GithubUser GetGithubUser(int githubId) => 
            dbContext.GithubUsers.FirstOrDefault(u => u.GithubId == githubId);

        public bool IsGithubUserRegistered(int githubId) => 
            dbContext.GithubUsers.Any(u => u.GithubId == githubId);
    }
}