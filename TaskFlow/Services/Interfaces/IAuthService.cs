using TaskFlow.DTOs.Auth;

namespace TaskFlow.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthenticationResponse> LoginAsync(AuthenticationRequest request);
        Task<AuthenticationResponse> RefreshAsync(TokenRequest request);
        Task LogoutAsync(string refreshToken);

    }
}
