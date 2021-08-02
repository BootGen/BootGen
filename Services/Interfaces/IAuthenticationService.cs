using Editor;

namespace Editor.Services
{
    public interface IAuthenticationService
    {
        LoginResponse Login(AuthenticationData data);
        OAuthLoginResponse GithubOAuthLogin(OAuthUser githubUser, string accessToken);
    }
}
