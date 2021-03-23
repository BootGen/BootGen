using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Editor.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        protected BaseController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        protected User CurrentUser
        {
            get
            {
                string value = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(value))
                    return null;
                int id = int.Parse(value);
                return dbContext.Users.Where(u => u.Id == id).FirstOrDefault();
            }
        }
    }
}