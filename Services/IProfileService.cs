namespace Editor.Services
{
    public interface IProfileService
    {
        User CurrentUser { get; set; }
        ProfileResponse UpdateProfile(User user);
        bool ChangePassword(ChangePasswordData data);
    }
}
