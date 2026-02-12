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
                                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                                .AddJwtBearer(opt =>
                                {
                                        opt.TokenValidationParameters = new TokenValidationParameters
                                        {
                                                ValidateIssuer = true,
                                                ValidIssuer = issuer,

                                                ValidateAudience = true,
                                                ValidAudience = audience,

                                                ValidateIssuerSigningKey = true,
                                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),

                                                ValidateLifetime = true,
                                                ClockSkew = TimeSpan.FromSeconds(30)
                                        };
                                });

                        builder.Services.AddAuthorization();

                        var app = builder.Build();

                        app.UseSwagger();
                        app.UseSwaggerUI();

                        app.UseHttpsRedirection();

                        app.UseAuthentication();
                        app.UseAuthorization();

                        app.MapControllers();

                        app.Run();
                }
        }
}