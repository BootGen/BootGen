namespace Editor.Services.Interfaces
{
    public interface IOAuthService
    {
        OAuthUser GetGithubUser(int githubId);
        bool IsGithubUserRegistered(int githubId);
    }
}