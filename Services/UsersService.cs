using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebProject.Services
{
    public class UsersService : IUsersService
    {

        public ServiceResponse<List<User>> GetUsers()
        {
            using (var db = new DataContext())
            {
                var query = db.Users;
                return new ServiceResponse<List<User>>
                {
                    StatusCode = 200,
                    ResponseData = query.ToList()
                };
            }
        }

        public ServiceResponse<User> GetUser(int userId)
        {
            using (var db = new DataContext())
            {
                var item = db.Users
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
}
