using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace ChatApp.WebAPI.Controllers
{
    [ApiController]
    public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        protected int GetUserId()
        {
            var hasValue = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId);
            return hasValue ? userId : -1;
        }
        protected string GetUserName()
        {
            return User.FindFirst(ClaimTypes.Name).Value;
        }
    }
}
