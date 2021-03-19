using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Editor.Services
{
    public class RegistrationService : IRegistrationService
    {
        public ServiceResponse<ProfileResponse> CheckRegistration(RegistrationData data)
        {
            using (var db = new DataContext())
            {
                bool isEmailInUse = db.Users.Where(u => u.Email == data.Email).Any();
                bool isUserNameInUse = db.Users.Where(u => u.UserName == data.UserName).Any();
                return new ServiceResponse<ProfileResponse>
                {
                    StatusCode = 200,
                    ResponseData = new ProfileResponse
                    {
                        Success = !isUserNameInUse && !isEmailInUse,
                        IsUserNameInUse = isUserNameInUse,
                        IsEmailInUse = isEmailInUse
                    }
                };
            }
        }

        public ServiceResponse<ProfileResponse> Register(RegistrationData data)
        {
            var response = CheckRegistration(data);
            if (response.ResponseData.Success)
            {
                using (var db = new DataContext())
                {
                    User newUser = new User { UserName = data.UserName, Email = data.Email };
                    newUser.PasswordHash = new PasswordHasher<User>().HashPassword(newUser, data.Password);
                    db.Users.Add(newUser);
                    db.SaveChanges();
                }
            }
            return response;
        }
    }
}
