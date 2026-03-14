using TaskFlow.DTOs.Pagination;
using TaskFlow.DTOs.User;

namespace TaskFlow.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<UserListResponse>> GetPagedUsersAsync(UserFilterRequest filter);
        Task<UserDetailsResponse> GetUserByIdAsync(int id);
        Task<UserDetailsResponse> CreateUserAsync(UserRequest dto);
        Task<UserDetailsResponse> UpdateUserAsync(int id, UserRequest dto);
        Task CancelUserAsync(int id);
    }
}
