using System;
using System.Linq;
using System.Threading.Tasks;
using Editor.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Editor.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfiguration configuration;
        private readonly IGithubService githubService;
        private readonly IOAuthService oAuthService;

        public RegistrationService(ApplicationDbContext dbContext, IConfiguration configuration, IGithubService githubService, IOAuthService oAuthService)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
            this.githubService = githubService;
            this.oAuthService = oAuthService;
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

        public ProfileResponse CheckRegistration(RegistrationData data)
        {
            bool isEmailInUse = dbContext.Users.Where(u => u.Email == data.Email).Any();
            bool isUserNameInUse = dbContext.Users.Where(u => u.UserName == data.UserName).Any();
            return new ProfileResponse
            {
                Success = !isUserNameInUse && !isEmailInUse,
                IsUserNameInUse = isUserNameInUse,
                IsEmailInUse = isEmailInUse
            };
        }

        public ProfileResponse Register(RegistrationData data)
        {
            var response = CheckRegistration(data);
            if (response.Success)
            {
                User newUser = new User
                {
                    UserName = data.UserName,
                    Email = data.Email,
                    Newsletter = data.Newsletter,
                    IsActive = false,
                    ActivationToken = Guid.NewGuid().ToString()
                };
                newUser.PasswordHash = new PasswordHasher<User>().HashPassword(newUser, data.Password);
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                var apiKey = configuration["SendGridApiKey"];
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("info@bootgen.com", "BootGen");
                    var subject = "confirm e-mail address";
                    var to = new EmailAddress(newUser.Email, newUser.UserName);
                    var plainTextContent = $"Hi {newUser.UserName},\nThank you for registering at bootgen.com! To confirm your e-mail address please click on the following link:\nhttps://bootgen.com/activation/{newUser.ActivationToken}\nBest Regards,\nThe BootGen Team";
                    var htmlContent = $"<strong>Hi {newUser.UserName},</strong><br>Thank you for registering at bootgen.com! To confirm your e-mail address please click on the following link:<br><a href=\"https://bootgen.com/activation/{newUser.ActivationToken}\" target=\"_blank\">https://bootgen.com/activation/{newUser.ActivationToken}</a><br>Best Regards,<br>The BootGen Team";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    client.SendEmailAsync(msg);
                }
            }
            return response;
        }

        public async Task<OAuthUser> RegisterViaGithub(GithubRegistrationData data, GithubUserInfo githubUserInfo)
        {
            var githubUser = new OAuthUser();

            if (!oAuthService.IsGithubUserRegistered(githubUserInfo.Id))
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == githubUserInfo.Email);
                if (user == null)
                {
                    user = dbContext.Users.Add(new User
                    {
                        UserName = githubUserInfo.Login,
                        Email = githubUserInfo.Email,
                        IsActive = true,
                        ActivationToken = null
                    }).Entity;
                }
                githubUser = new OAuthUser
                {
                    User = user,
                    OAuthId = githubUserInfo.Id,
                    Email = githubUserInfo.Email,
                    Login = githubUserInfo.Login
                };
                dbContext.OAuthUsers.Add(githubUser);

                await dbContext.SaveChangesAsync();
            }

            return githubUser;
        }
    }
}
