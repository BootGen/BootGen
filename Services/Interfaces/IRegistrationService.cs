using System.Threading.Tasks;

namespace Editor.Services
{
    public interface IRegistrationService
    {
        ProfileResponse Register(RegistrationData data);
        bool Activate(string activationCode);
        Task<GithubUser> RegisterViaGithub(GithubRegistrationData data, GithubUserInfo githubUserInfo);
    }
}
