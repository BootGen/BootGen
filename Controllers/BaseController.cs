using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WebProject.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected User CurrentUser
        {
            get
            {
                string value = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(value))
                    return null;
                int id = int.Parse(value);
                using (var db = new DataContext())
                {
                    return db.Users.Where(u => u.Id == id).FirstOrDefault();
                }
            }
        }
    }
}