using TaskFlow.DTOs.TaskItem;

namespace TaskFlow.Services.Interfaces
{
    public interface ITaskItemService
    {
        Task<List<TaskItemListResponse>> GetAllTaskItemAsync();
        Task<TaskItemListResponse> GetTaskItemByIdAsync(int id);
        Task<TaskItemListResponse> CreateTaskItemAsync(CreateTaskItemRequest dto);
        Task<TaskItemListResponse> UpdateTaskItemAsync(UpdateTaskItemRequest dto);
        Task CancelTaskItemAsync(int id);
        Task DeleteTaskItemAsync(int id);
    }
}
