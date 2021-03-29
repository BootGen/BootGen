using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace Editor.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration config;
        private readonly ApplicationDbContext dbContext;

        public User CurrentUser { get; set; }

        public AuthenticationService(IConfiguration config, ApplicationDbContext dbContext)
        {
            this.config = config;
            this.dbContext = dbContext;
        }
        public LoginResponse Login(AuthenticationData data)
        {
            User user = dbContext.Users.Where(u => u.Email == data.Email).FirstOrDefault();
            if (user != null && new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, data.Password) != PasswordVerificationResult.Failed)
            {
                if (!user.IsActive)
                    return new LoginResponse
                    {
                        Success = false,
                        IsInactive = true
                    };
                return new LoginResponse
                {
                    Success = true,
                    Jwt = GenerateJSONWebToken(user)
                };
            }
            return new LoginResponse
            {
                Success = false,
                WrongCreditentials = true
            };
        }

        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
              config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);
            token.Payload["user"] = user;

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}