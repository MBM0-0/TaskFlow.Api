using TaskFlow.Models;

namespace TaskFlow.Repositories.Interfaces
{
    public interface ITaskItemRepository
    {
        Task<List<TaskItem>> GetAllAsync();
        Task<TaskItem> GetByIdAsync(int id);
        Task AddAsync(TaskItem entity);
        Task DeleteAsync(TaskItem entity);
        Task SaveChangesAsync();

    }
}
