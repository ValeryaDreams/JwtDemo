using System.Security.Cryptography;
using System.Text;

namespace JwtDemo.Auth
{
        public class RefreshTokenService(IConfiguration config)
        {
                public (string rawToken, RefreshTokenRecord record) Create(string userId)
                {
                        var days = int.Parse(config.GetSection("Jwt")["RefreshTokenDays"]!);

                        // Отдача клиенту.
                        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                        // Хранение на сервере.
                        var hash = Sha256(raw);

                        var now = DateTime.UtcNow;

                        return (raw, new RefreshTokenRecord
                        {
                                UserId = userId,
                                TokenHash = hash,
                                CreatedAtUtc = now,
                                ExpreseAtUtc = now.AddDays(days),
                        });
                }

                public string Hash(string rawToken)
                {
                        return Sha256(rawToken);
                }

                private static string Sha256(string input)
                {
                        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

                        return Convert.ToHexString(bytes);
                }
        }
}
