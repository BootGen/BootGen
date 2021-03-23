using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Editor.Services
{
    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext dbContext;

        public UsersService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ServiceResponse<List<User>> GetUsers()
        {
            var query = dbContext.Users;
            return new ServiceResponse<List<User>>
            {
                StatusCode = 200,
                ResponseData = query.ToList()
            };
        }

        public ServiceResponse<User> GetUser(int userId)
        {
            var item = dbContext.Users
                         .Where(item => item.Id == userId).FirstOrDefault();
            if (item == null)
                return new ServiceResponse<User>
                {
                    StatusCode = 404
                };
            return new ServiceResponse<User>
            {
                StatusCode = 200,
                ResponseData = item
            };
        }
    }
}
