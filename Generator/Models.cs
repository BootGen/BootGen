using System.Collections.Generic;

namespace Generator
{
    [Readonly]
    [Authenticate]
    public class User
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        [ServerOnly]
        public string PasswordHash { get; set; }
        [OneToMany(parentName: "Owner")]
        public List<Project> Projects { get; set; }
    }

    //Authentication
    class AuthenticationData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    class LoginResponse
    {
        public string Jwt { get; set; }
    }

    interface Authentication
    {
        [Post]
        LoginResponse Login(AuthenticationData data);
    }

    //Registration
    class RegistrationData
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    class ProfileResponse
    {
        public bool Success { get; set; }
        public bool IsUserNameInUse { get; set; }
        public bool IsEmailInUse { get; set; }
    }

    interface Registration
    {
        [Post]
        ProfileResponse Register(RegistrationData data);
    }

    //Profile
    public class ChangePasswordData
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    [Authenticate]
    interface Profile
    {
        [Get]
        User Profile();

        [Post]
        ProfileResponse UpdateProfile(User user);

        [Post]
        bool ChangePassword(ChangePasswordData data);
    }

    class GenerateRequest
    {
        public string Data { get; set; }
    }

    class GeneratedFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
    }

    class GenerateResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<GeneratedFile> GeneratedFiles { get; set; }
    }

    interface Generate
    {
        [Post]
        GenerateResponse Generate(GenerateRequest request);
    }

    public class Project
    {
        public string Name { get; set; }
        public string Json { get; set; }
    }
}
