using TaskFlow.DTOs.Auth;

namespace TaskFlow.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthenticationResponse> LoginAsync(AuthenticationRequest request);
    }
}
