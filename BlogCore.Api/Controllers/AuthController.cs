using BlogCore.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult IsAuthenticated(AdminUser adminUser)
        {
            bool status = false;
            if (adminUser.Email == "umutatras@gmail.com" && adminUser.Password == "admin")
            {
                status = true;
            }
            var result = new
            {
                status = status
            };
            return Ok(result);
        }
    }

}
