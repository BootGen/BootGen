namespace Editor.Services
{
    public interface IAuthenticationService
    {
        LoginResponse Login(AuthenticationData data);
    }
}
