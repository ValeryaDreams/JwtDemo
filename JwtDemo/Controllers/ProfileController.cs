using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDemo.Controllers
{
        [ApiController]
        [Route("profile")]
        public class ProfileController : ControllerBase
        {
                [HttpGet]
                [Authorize]
                public IActionResult Me()
                {
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier ?? User.FindFirstValue("sub"));
                        var email = User.FindFirstValue(ClaimTypes.Email ?? User.FindFirstValue("email"));
                        var roles = User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();

                        return Ok(new { userId, email, roles });
                }

                [HttpGet("admin-only")]
                [Authorize(Roles = "admin")]
                public IActionResult AdminOnly() => Ok("You are admin");
        }
}
