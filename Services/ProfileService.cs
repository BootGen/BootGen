using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Editor.Services
{
    public class ProfileService : IProfileService
    {
        public User CurrentUser { get; set; }

        public ServiceResponse<ChangePasswordResponse> ChangePassword(ChangePasswordData data)
        {
            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(CurrentUser, CurrentUser.PasswordHash, data.OldPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return new ServiceResponse<ChangePasswordResponse>
                {
                    StatusCode = 401,
                    ResponseData = new ChangePasswordResponse { Value = false }
                };
            }
            using (var db = new DataContext())
            {
                CurrentUser.PasswordHash = passwordHasher.HashPassword(CurrentUser, data.NewPassword);
                db.Update(CurrentUser);
                db.SaveChanges();
            }
            return new ServiceResponse<ChangePasswordResponse>
            {
                StatusCode = 200,
                ResponseData = new ChangePasswordResponse { Value = true }
            };
        }

        public ServiceResponse<ProfileResponse> CheckProfile(User user)
        {
            using (var db = new DataContext())
            {
                bool isEmailInUse = db.Users.Where(u => u.Id != CurrentUser.Id && u.Email == user.Email).Any();
                bool isUserNameInUse = db.Users.Where(u => u.Id != CurrentUser.Id && u.UserName == user.UserName).Any();
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

        public ServiceResponse<User> Profile()
        {
            return new ServiceResponse<User>
            {
                StatusCode = 200,
                ResponseData = CurrentUser
            };
        }

        public ServiceResponse<ProfileResponse> UpdateProfile(User user)
        {
            var response = CheckProfile(user);
            if (response.ResponseData.Success)
            {
                using (var db = new DataContext())
                {
                    CurrentUser.Email = user.Email;
                    CurrentUser.UserName = user.UserName;
                    db.Update(CurrentUser);
                    db.SaveChanges();
                }
            }
            return response;
        }
    }
}
