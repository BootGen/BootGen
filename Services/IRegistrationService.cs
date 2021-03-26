namespace Editor.Services
{
    public interface IRegistrationService
    {
        ServiceResponse<ProfileResponse> Register(RegistrationData data);
        bool Activate(string activationCode);
    }
}
