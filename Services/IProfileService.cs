namespace Editor.Services
{
    public interface IProfileService
    {
        User CurrentUser { get; set; }
        ServiceResponse<User> Profile();
        ServiceResponse<ProfileResponse> UpdateProfile(User user);
        ServiceResponse<ChangePasswordResponse> ChangePassword(ChangePasswordData data);
    }
}
