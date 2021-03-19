namespace Editor.Services
{
    public interface IAuthenticationService
    {
        ServiceResponse<LoginResponse> Login(AuthenticationData data);
    }
}
