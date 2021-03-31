namespace Editor.Services
{
    public interface IRegistrationService
    {
        ProfileResponse Register(RegistrationData data);
        bool Activate(string activationCode);
    }
}
