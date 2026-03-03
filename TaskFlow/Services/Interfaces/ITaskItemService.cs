using TaskFlow.DTOs.TaskItem;
using TaskFlow.Models;

namespace TaskFlow.Services.Interfaces
{
    public interface ITaskItemService
    {
        Task<List<TaskItemListResponse>> GetAllTaskItemAsync();
        Task<TaskItemListResponse> GetTaskItemByIdAsync(int id);
        Task<TaskItemListResponse> CreateTaskItemAsync(TaskItemRequest dto, int userId);
        Task<TaskItemListResponse> UpdateTaskItemAsync(int id, TaskItemRequest dto);
        Task CancelTaskItemAsync(int id);
        Task DeleteTaskItemAsync(int id);
    }
}
