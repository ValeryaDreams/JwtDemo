using Microsoft.AspNetCore.Mvc;
using JwtDemo.Auth;

namespace JwtDemo.Controllers
{
        [ApiController]
        [Route("auth")]
        public class AuthController(JwtTokenService tokens) : ControllerBase
        {
                public record LoginRequest(string Email, string Password);

                [HttpPost("login")]
                public IActionResult Login(LoginRequest req)
                {
                        if (req.Email == "kraken@paw.mewo" && req.Password == "shrimp")
                        {
                                // Генерация JWT.
                                var jwt = tokens.CreateAccessToken("1", req.Email, ["cat"]);
                                // Возврат токена.
                                return Ok(new { access_token = jwt });
                        }

                        if (req.Email == "catpuff13337@gmail.com" && req.Password == "chocopoko")
                        {
                                var jwt = tokens.CreateAccessToken("2", req.Email, ["user"]);
                                return Ok(new { access_token = jwt });
                        }

                        if (req.Email == "1997valerya1997@gmail.com" && req.Password == "1234")
                        {
                                var jwt = tokens.CreateAccessToken("3", req.Email, ["admin"]);
                                return Ok(new { access_token = jwt });
                        }

                        return Unauthorized(new { error = "Invalid credentials" });
                }

        }
}