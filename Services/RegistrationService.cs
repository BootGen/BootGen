using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Editor.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ApplicationDbContext dbContext;

        public RegistrationService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool Activate(string activationToken)
        {
            var user = dbContext.Users.FirstOrDefault(u => !u.IsActive && u.ActivationToken == activationToken);
            if (user == null)
                return false;
            dbContext.Update(user);
            user.IsActive = true;
            user.ActivationToken = null;
            dbContext.SaveChanges();
            return true;
        }

        public ServiceResponse<ProfileResponse> CheckRegistration(RegistrationData data)
        {
            bool isEmailInUse = dbContext.Users.Where(u => u.Email == data.Email).Any();
            bool isUserNameInUse = dbContext.Users.Where(u => u.UserName == data.UserName).Any();
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

        public ServiceResponse<ProfileResponse> Register(RegistrationData data)
        {
            var response = CheckRegistration(data);
            if (response.ResponseData.Success)
            {
                User newUser = new User {
                    UserName = data.UserName,
                    Email = data.Email,
                    IsActive = false,
                    ActivationToken = Guid.NewGuid().ToString()
                };
                newUser.PasswordHash = new PasswordHasher<User>().HashPassword(newUser, data.Password);
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
            }
            return response;
        }
    }
}
