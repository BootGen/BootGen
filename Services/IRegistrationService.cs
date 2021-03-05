namespace WebProject.Services
{
    public interface IRegistrationService
    {
        ServiceResponse<ProfileResponse> Register(RegistrationData data);
    }
}
