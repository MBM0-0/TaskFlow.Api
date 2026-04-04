using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskFlow.Data;
using TaskFlow.DTOs.Auth;
using TaskFlow.Extensions.Middlewares.Exceptions;
using TaskFlow.Models;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly AppDbContext _dbcontext;
        private readonly PasswordHasher<User> _passwordHasher = new();
        private readonly ILogger<AuthService> _logger;


        public AuthService(JwtOptions jwtOptions, AppDbContext dbcontext, ILogger<AuthService> logger)
        {
            _jwtOptions = jwtOptions;
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task<AuthenticationResponse> LoginAsync(AuthenticationRequest request)
        {
            var user = await _dbcontext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName == request.Username);
            if (user == null)
            {
                throw new UnauthorizedException("Invalid username or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedException("Invalid username or password");
            }

            var oldTokens = await _dbcontext.RefreshTokens.Where(rt => rt.UserId == user.Id && !rt.IsRevoked).ToListAsync();
            foreach (var t in oldTokens)
            {
                t.IsRevoked = true;
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

            var refreshToken = GenerateRefreshToken();

            _dbcontext.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = Hash(refreshToken),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            });

            await _dbcontext.SaveChangesAsync();
            _logger.LogInformation("User {UserId} logged in successfully", user.Id);
            return new AuthenticationResponse
            {
                AccessToken = jwt,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = tokenDescriptor.Expires.Value
            };
        }

        public async Task<AuthenticationResponse> RefreshAsync(TokenRequest request)
        {
            var hashed = Hash(request.RefreshToken);

            var storedToken = await _dbcontext.RefreshTokens.Include(rt => rt.User).ThenInclude(u => u.Role).FirstOrDefaultAsync(rt => rt.TokenHash == hashed && !rt.IsRevoked);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate <= DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid refresh token used");
                throw new UnauthorizedException("Invalid refresh token");
            }

            var user = storedToken.User;

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
            var newAccessToken = tokenHandler.WriteToken(token);

            storedToken.IsRevoked = true;

            var newRefreshToken = GenerateRefreshToken();

            _dbcontext.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = Hash(newRefreshToken),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            });

            await _dbcontext.SaveChangesAsync();

            return new AuthenticationResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiresAt = tokenDescriptor.Expires.Value
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var hashed = Hash(refreshToken);

            var token = await _dbcontext.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == hashed);

            if (token != null)
            {
                token.IsRevoked = true;
                await _dbcontext.SaveChangesAsync();
            }
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string Hash(string input)
        {
            using var hash = SHA256.Create();
            var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }

}
