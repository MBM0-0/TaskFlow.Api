using TaskFlow.DTOs.Pagination;
using TaskFlow.DTOs.TaskItem;
using TaskFlow.Models;

namespace TaskFlow.Services.Interfaces
{
    public interface ITaskItemService
    {
        Task<PagedResponse<TaskItemListResponse>> GetPagedTaskItemsAsync(TaskItemFilterRequest filter);
        Task<TaskItemListResponse> GetTaskItemByIdAsync(int id);
        Task<TaskItemListResponse> CreateTaskItemAsync(TaskItemRequest dto, int userId);
        Task<TaskItemListResponse> UpdateTaskItemAsync(int id, TaskItemRequest dto);
        Task CancelTaskItemAsync(int id);
        Task DeleteTaskItemAsync(int id);
    }
}
