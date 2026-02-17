using Mapster;
using TaskFlow.DTOs.TaskItem;
using TaskFlow.Models;
using TaskFlow.Repositories.Interfaces;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _repository;

        public TaskItemService(ITaskItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaskItemListResponse>> GetAllTaskItemAsync()
        {
            var entity = await _repository.GetAllAsync();
            return entity.Adapt<List<TaskItemListResponse>>();
        }

        public async Task<TaskItemListResponse> GetTaskItemByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity.Adapt<TaskItemListResponse>();
        }

        public async Task<TaskItemListResponse> CreateTaskItemAsync(CreateTaskItemRequest dto)
        {
            var entity = dto.Adapt<TaskItem>();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity.Adapt<TaskItemListResponse>();
        }

        public async Task<TaskItemListResponse> UpdateTaskItemAsync(UpdateTaskItemRequest dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            entity.UpdatedAt = DateTime.UtcNow;

            entity.Id = dto.Id;
            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.Status = dto.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return entity.Adapt<TaskItemListResponse>();
        }

        public async Task CancelTaskItemAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            entity.DeletedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
        }
        public async Task DeleteTaskItemAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            await _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync();
        }
    }
}

