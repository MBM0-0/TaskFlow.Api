using TaskFlow.DTOs.User;
using TaskFlow.Models;

namespace TaskFlow.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<(List<User> Users, int TotalCount)> GetPagedAsync(UserFilterRequest query);
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User entity);
        Task<bool> IsUserNameFoundAsync(string UserName);
        Task SaveChangesAsync();
        Task<bool> RoleExistsAsync(int roleId);

    }
}
