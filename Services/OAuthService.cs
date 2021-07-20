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

        public OAuthUser GetGithubUser(int githubId) => 
            dbContext.OAuthUsers.FirstOrDefault(u => u.OAuthId == githubId);

        public bool IsGithubUserRegistered(int githubId) => 
            dbContext.OAuthUsers.Any(u => u.OAuthId == githubId);
    }
}