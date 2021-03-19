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
    public interface IUsersService
    {
        ServiceResponse<List<User>> GetUsers();
        ServiceResponse<User> GetUser(int userId);
    }
}
