using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.Data;
using TaskFlow.DTOs.Auth;
using TaskFlow.Middlewares.Exceptions;
using TaskFlow.Models;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly AppDbContext _dbcontext;

        public AuthService(JwtOptions jwtOptions, AppDbContext dbcontext)
        {
            _jwtOptions = jwtOptions;
            _dbcontext = dbcontext;
        }

        public async Task<AuthenticationResponse> LoginAsync(AuthenticationRequest request)
        {
            var user = await _dbcontext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName == request.Username && u.PasswordHash == request.Password);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid username or password");
            }

            var key = Encoding.UTF8.GetBytes(_jwtOptions.SigningKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role.Name)
                })
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return new AuthenticationResponse
            {
                Token = jwt,
                ExpiresAt = tokenDescriptor.Expires ?? DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTime)
            };
        }
    }
}
