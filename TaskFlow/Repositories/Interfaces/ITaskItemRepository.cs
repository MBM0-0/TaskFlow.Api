using TaskFlow.DTOs.TaskItem;
using TaskFlow.Models;

namespace TaskFlow.Repositories.Interfaces
{
    public interface ITaskItemRepository
    {
        Task<(List<TaskItem> TaskItems, int totalCount)> GetPagedAsync(TaskItemFilterRequest query);
        Task<TaskItem> GetByIdAsync(int id);
        Task AddAsync(TaskItem entity);
        Task DeleteAsync(TaskItem entity);
        Task SaveChangesAsync();

    }
}
