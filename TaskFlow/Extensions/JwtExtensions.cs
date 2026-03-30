using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TaskFlow.DTOs.Auth;

namespace TaskFlow.Extensions
{
    public static class JwtExtensions
    {
        public static void AddTaskFlowJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();
            services.AddSingleton(jwtOptions);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.SigningKey)
                        ),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role
                    };
                });
        }
    }
}
