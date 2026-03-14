using Mapster;
using TaskFlow.DTOs.Pagination;
using TaskFlow.DTOs.TaskItem;
using TaskFlow.Enums;
using TaskFlow.Middlewares.Exceptions;
using TaskFlow.Models;
using TaskFlow.Repositories.Interfaces;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _repository;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;

        public TaskItemService(ITaskItemRepository repository, IUserService userService, IProjectService projectService)
        {
            _repository = repository;
            _userService = userService;
            _projectService = projectService;
        }

        public async Task<PagedResponse<TaskItemListResponse>> GetPagedTaskItemsAsync(TaskItemFilterRequest filter)
        {
            var (TaskItem, TotalCount) = await _repository.GetPagedAsync(filter);

            return new PagedResponse<TaskItemListResponse>
            {
                Items = TaskItem.Adapt<List<TaskItemListResponse>>(),
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = TotalCount
            };
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

        public async Task<TaskItemListResponse> CreateTaskItemAsync(TaskItemRequest dto, int userId)
        {
            var entity = dto.Adapt<TaskItem>();
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new BadRequestException("Task title is required.");
            }
            if (dto.Status < 0 || (int)dto.Status >= Enum.GetValues(typeof(TaskItemStatus)).Length)
            {
                throw new BadRequestException("Invalid task status.");
            }

            await _userService.GetUserByIdAsync(dto.AssignedToUserId);
            await _projectService.GetProjectByIdAsync(dto.ProjectId);

            entity.CreatedByUserId = userId;
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity.Adapt<TaskItemListResponse>();
        }

        public async Task<TaskItemListResponse> UpdateTaskItemAsync(int id, TaskItemRequest dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Task item not found.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Title) && dto.Title != entity.Title)
            {
                entity.Title = dto.Title;
            }

            if (dto.Status < 0 || (int)dto.Status >= Enum.GetValues(typeof(TaskItemStatus)).Length)
            {
                throw new BadRequestException("Invalid task status.");
            }

            if (dto.AssignedToUserId > 0 && dto.AssignedToUserId != entity.AssignedToUserId)
            {
                await _userService.GetUserByIdAsync(dto.AssignedToUserId);
                entity.AssignedToUserId = dto.AssignedToUserId;
            }
            if (dto.ProjectId > 0 && dto.ProjectId != entity.ProjectId)
            {
                await _projectService.GetProjectByIdAsync(dto.ProjectId);
                entity.ProjectId = dto.ProjectId;
            }

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
                throw new NotFoundException("Task item not found or already canceled..");
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

