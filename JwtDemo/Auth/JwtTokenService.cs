using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtDemo.Auth
{
        public class JwtTokenService(IConfiguration config)
        {
                public string CreateAccessToken(string userId, string email, string[] roles)
                {
                        var jwtSection = config.GetSection("Jwt");
                        var issuer = jwtSection["Issuer"];
                        var audience = jwtSection["Audience"];
                        var key = jwtSection["Key"];
                        var minutes = int.Parse(jwtSection["AccessTokenMinutes"]!);

                        // Формирование claims (payload).
                        var claims = new List<Claim>
                        {
                                new(JwtRegisteredClaimNames.Sub, userId),
                                new(JwtRegisteredClaimNames.Email, email),
                                // Уникальный ID токена.
                                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };

                        foreach (var role in roles)
                        {
                                claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
                        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                                issuer: issuer,
                                audience: audience,
                                claims: claims,
                                notBefore: DateTime.UtcNow,
                                expires: DateTime.UtcNow.AddMinutes(minutes),
                                signingCredentials: creds
                         );

                        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                        var handler = new JwtSecurityTokenHandler();
                        var decoded = handler.ReadJwtToken(tokenString);

                        Console.WriteLine("HEADER");
                        foreach (var item in decoded.Header)
                        {
                                Console.WriteLine($"{item.Key}: {item.Value}");
                        }

                        Console.WriteLine("PAYLOAD");
                        foreach (var item in decoded.Claims)
                        {
                                Console.WriteLine($"{item.Type}: {item.Value}");
                        }

                        return tokenString;
                }
        }
}