using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebProject.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace WebProject.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private IConfiguration _config;
        public User CurrentUser { get; set; }

        public AuthenticationService(IConfiguration config)
        {
            _config = config;
        }
        public ServiceResponse<LoginResponse> Login(AuthenticationData data)
        {
            User user = null;
            using (var db = new DataContext())
            {
                user = db.Users.Where(u => u.Email == data.Email).FirstOrDefault();
            }
            if (user != null && new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, data.Password) != PasswordVerificationResult.Failed)
            {
                return new ServiceResponse<LoginResponse>
                {
                    StatusCode = 200,
                    ResponseData = new LoginResponse
                    {
                        Jwt = GenerateJSONWebToken(user)
                    }
                };
            }
            return new ServiceResponse<LoginResponse>
            {
                StatusCode = 401,
                ResponseData = new LoginResponse()
            };
        }

        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);
            token.Payload["user"] = user;

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}