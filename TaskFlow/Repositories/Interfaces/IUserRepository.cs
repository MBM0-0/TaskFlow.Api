using TaskFlow.Models;

namespace TaskFlow.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User entity);
        Task<bool> IsUserNameFoundAsync(string UserName);
        Task SaveChangesAsync();
    }
}
