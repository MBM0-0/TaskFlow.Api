using TaskFlow.DTOs.Pagination;
using TaskFlow.DTOs.User;
using TaskFlow.Models;

namespace TaskFlow.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<UserListResponse>> GetPagedUsersAsync(UserFilterRequest filter);
        Task<UserDetailsResponse> GetUserByIdAsync(int id);
        Task<UserDetailsResponse> CreateUserAsync(UserRequest dto, int userId);
        Task<UserDetailsResponse> UpdateUserAsync(int id, UserRequest dto, int userId);
        Task CancelUserAsync(int id, int userId);
    }
}
