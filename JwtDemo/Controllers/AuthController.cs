using Microsoft.AspNetCore.Mvc;
using JwtDemo.Auth;

namespace JwtDemo.Controllers
{
        [ApiController]
        [Route("auth")]
        public class AuthController(JwtTokenService tokens,
                                    RefreshTokenService refreshToken,
                                    InMemoryRefreshTokenStore inMemory) : ControllerBase
        {
                public record LoginRequest(string Email, string Password);
                public record RefrreshRequest(string? RefreshToken);

                [HttpPost("login")]
                public IActionResult Login(LoginRequest req)
                {
                        if (req.Email == "kraken@paw.mewo" && req.Password == "shrimp")
                        {
                                // Возврат токена.
                                return IssueToken(userId: "1", email: req.Email, roles: ["cat"]);
                        }

                        if (req.Email == "catpuff13337@gmail.com" && req.Password == "chocopoko")
                        {
                                return IssueToken(userId: "2", email: req.Email, roles: ["user"]);
                        }

                        if (req.Email == "1997valerya1997@gmail.com" && req.Password == "1234")
                        {
                                return IssueToken(userId: "3", email: req.Email, roles: ["admin"]);
                        }

                        return Unauthorized(new { error = "Invalid credentials" });
                }

                [HttpPost("refresh")]
                public IActionResult Refresh([FromBody] RefrreshRequest? body)
                {
                        // Взять токен ил куки.
                        var raw = Request.Cookies["refresh_token"] ?? body?.RefreshToken;

                        if (string.IsNullOrWhiteSpace(raw))
                        {
                                return Unauthorized(new { error = "No refresh token" });
                        }

                        // Хеширование + поиск на сервере
                        var hash = refreshToken.Hash(raw);
                        var existing = inMemory.FindByHash(hash);

                        if (existing is null || existing.IsRevoked || existing.IsExpired)
                        {
                                return Unauthorized(new { error = "Invalid refresh token" });
                        }

                        // Страрый токен отзвать, а новый выдать.
                        existing.RevokedAtUtc = DateTime.UtcNow;

                        var (newRaw, newRecord) = refreshToken.Create(existing.UserId);
                        existing.ReplacedByTokenHash = newRecord.TokenHash;
                        inMemory.Save(newRecord);

                        // Создание нового access Токена.
                        var (email, roles) = existing.UserId ==
                                "1" ? ("1997valerya1997@gmail.com", new[] { "admin" }) :
                                      ("kraken@paw.mewo", new[] { "cat" });

                        var access = tokens.CreateAccessToken(existing.UserId, email, roles);

                        SetRefreshCookie(newRaw);

                        return Ok(new { access_token = access });
                }

                [HttpPost("logout")]
                public IActionResult Logout()
                {
                        Response.Cookies.Delete("refresh_token");
                        return Ok();
                }

                private IActionResult IssueToken(string userId, string email, string[] roles)
                {
                        var access = tokens.CreateAccessToken(userId, email, roles);

                        var (rawRefresh, record) = refreshToken.Create(access);
                        inMemory.Save(record);

                        SetRefreshCookie(rawRefresh);

                        return Ok(new { access_token = access });
                }

                private void SetRefreshCookie(string rawRefresh)
                {
                        Response.Cookies.Append("refresh_token", rawRefresh, new CookieOptions
                        {
                                HttpOnly= true,
                                Secure= true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTimeOffset.UtcNow.AddDays(14)
                        });
                }

        }
}