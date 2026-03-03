using TaskFlow.DTOs.User;

namespace TaskFlow.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserListResponse>> GetAllUserAsync();
        Task<UserDetailsResponse> GetUserByIdAsync(int id);
        Task<UserDetailsResponse> CreateUserAsync(UserRequest dto);
        Task<UserDetailsResponse> UpdateUserAsync(int id, UserRequest dto);
        Task CancelUserAsync(int id);
    }
}
