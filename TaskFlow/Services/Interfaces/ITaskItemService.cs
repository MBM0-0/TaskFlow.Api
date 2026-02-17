using TaskFlow.DTOs.TaskItem;

namespace TaskFlow.Services.Interfaces
{
    public interface ITaskItemService
    {
        Task<List<TaskItemListResponse>> GetAllTaskItemAsync();
        Task<TaskItemListResponse> GetTaskItemByIdAsync(int id);
        Task<TaskItemListResponse> CreateTaskItemAsync(TaskItemRequest dto);
        Task<TaskItemListResponse> UpdateTaskItemAsync(int id, TaskItemRequest dto);
        Task CancelTaskItemAsync(int id);
        Task DeleteTaskItemAsync(int id);
    }
}
