using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace JwtDemo
{
        public class Program
        {
                public static void Main(string[] args)
                {
                        var builder = WebApplication.CreateBuilder(args);
                        builder.Services.AddControllers();
                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen();

                        var jwtSection = builder.Configuration.GetSection("Jwt");
                        var issuer = jwtSection["Issuer"];
                        var audience = jwtSection["Audience"];
                        var key = jwtSection["Key"];

                        builder.Services
                                // asp.net используй beaver как основной способ аутентификации =>
                                // нужно искать токен в заголовке Authorization: Bearer xxx
                                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                                // Как именно проверять токен.
                                .AddJwtBearer(opt =>
                                {
                                        opt.TokenValidationParameters = new TokenValidationParameters
                                        {
                                                ValidateIssuer = true,
                                                ValidIssuer = issuer,

                                                ValidateAudience = true,
                                                ValidAudience = audience,

                                                ValidateIssuerSigningKey = true,
                                                // Проверка, что токен ПОДПИСАН нашим сервером.
                                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),                                                

                                                ValidateLifetime = true,
                                                ClockSkew = TimeSpan.FromSeconds(30)
                                        };
                                });

                        // JwtTokenService не хранит состояние, он только читает конфигурацию и генериует строку.
                        builder.Services.AddSingleton<Auth.JwtTokenService>();

                        builder.Services.AddAuthorization();                        

                        var app = builder.Build();

                        app.UseSwagger();
                        app.UseSwaggerUI();

                        // Автоматический редирект с http -> httpS.
                        app.UseHttpsRedirection();

                        // Читает JWT. Проверяет токен. Создает HttpContext.User.
                        app.UseAuthentication();
                        // Дает поддержку [Authorize].
                        // Смотрит [Authorize] => проверяет роли/политики.
                        app.UseAuthorization();

                        // ASP.NET ищет классы с [ApiController] и подключает их маршруты.
                        app.MapControllers();

                        app.Run();
                }
        }
}