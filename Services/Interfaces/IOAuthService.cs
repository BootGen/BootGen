namespace Editor.Services.Interfaces
{
    public interface IOAuthService
    {
        GithubUser GetGithubUser(int githubId);
        bool IsGithubUserRegistered(int githubId);
    }
}