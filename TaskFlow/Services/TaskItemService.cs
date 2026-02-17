using Mapster;
using TaskFlow.DTOs.TaskItem;
using TaskFlow.Middlewares.Exceptions;
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
            if (entity == null)
            {
                throw new NotFoundException("Task item not found.");
            }

            return entity.Adapt<TaskItemListResponse>();
        }

        public async Task<TaskItemListResponse> CreateTaskItemAsync(TaskItemRequest dto)
        {
            var entity = dto.Adapt<TaskItem>();
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ValidationException("Task title is required.");
            }
            if (dto.Status < 0 || (int)dto.Status >= Enum.GetValues(typeof(TaskStatus)).Length)
            {
                throw new ValidationException("Invalid task status.");
            }

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity.Adapt<TaskItemListResponse>();
        }

        public async Task<TaskItemListResponse> UpdateTaskItemAsync(int id, TaskItemRequest dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("There is No Task Item Found Whith This Id.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Title) && dto.Title != entity.Title)
            {
                entity.Title = dto.Title;
            }

            if (dto.Status < 0 || (int)dto.Status >= Enum.GetValues(typeof(TaskStatus)).Length)
            {
                throw new ValidationException("Invalid task status.");
            }

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
            if (entity == null || entity.DeletedAt != null)
            {
                throw new NotFoundException("Task item not found or Its already Canceld.");
            }

            entity.DeletedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
        }
        public async Task DeleteTaskItemAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Task item not found.");
            }

            await _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync();
        }
    }
}

