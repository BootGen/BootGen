using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Editor.Services
{
    public class ProfileService : IProfileService
    {
        public User CurrentUser { get; set; }

        private readonly ApplicationDbContext dbContext;

        public ProfileService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public bool ChangePassword(ChangePasswordData data)
        {
            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(CurrentUser, CurrentUser.PasswordHash, data.OldPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return false;
            }
            CurrentUser.PasswordHash = passwordHasher.HashPassword(CurrentUser, data.NewPassword);
            dbContext.Update(CurrentUser);
            dbContext.SaveChanges();
            return true;
        }

        public ProfileResponse CheckProfile(User user)
        {
            bool isEmailInUse = dbContext.Users.Where(u => u.Id != CurrentUser.Id && u.Email == user.Email).Any();
            bool isUserNameInUse = dbContext.Users.Where(u => u.Id != CurrentUser.Id && u.UserName == user.UserName).Any();
            return new ProfileResponse
                {
                    Success = !isUserNameInUse && !isEmailInUse,
                    IsUserNameInUse = isUserNameInUse,
                    IsEmailInUse = isEmailInUse
                };
        }

        public ProfileResponse UpdateProfile(User user)
        {
            var response = CheckProfile(user);
            if (response.Success)
            {
                CurrentUser.Email = user.Email;
                CurrentUser.UserName = user.UserName;
                CurrentUser.Newsletter = user.Newsletter;
                dbContext.Update(CurrentUser);
                dbContext.SaveChanges();
            }
            return response;
        }
    }
}
