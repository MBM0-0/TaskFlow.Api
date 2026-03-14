using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs.TaskItem;
using TaskFlow.Models;
using TaskFlow.Repositories.Interfaces;
using static System.Net.WebRequestMethods;

namespace TaskFlow.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly AppDbContext _dbcontext;

        public TaskItemRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<( List<TaskItem> TaskItems, int totalCount)> GetPagedAsync(TaskItemFilterRequest filter)
        {
            IQueryable<TaskItem> taskItemQuery = _dbcontext.TaskItems;

            if (filter.DeletedAt.HasValue)
            {
                taskItemQuery = filter.DeletedAt.Value
                  ? taskItemQuery.Where(x => x.DeletedAt != null)
                  : taskItemQuery.Where(x => x.DeletedAt == null);
            }

            if (filter.Status.HasValue && Enum.IsDefined(typeof(TaskStatus), filter.Status.Value))
            {
                taskItemQuery = taskItemQuery.Where(x => x.Status == filter.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                taskItemQuery = taskItemQuery.Where(x => x.Title.Contains(filter.Title));
            }

            if (filter.CreatedByUserId.HasValue)
            {
                taskItemQuery = taskItemQuery.Where(x => x.CreatedByUserId == filter.CreatedByUserId);
            }

            if (filter.AssignedToUserId.HasValue)
            {
                taskItemQuery = taskItemQuery.Where(x => x.AssignedToUserId == filter.AssignedToUserId);
            }

            taskItemQuery = filter.SortDesc ? taskItemQuery.OrderByDescending(x => x.CreatedAt) : taskItemQuery.OrderBy(x => x.CreatedAt);

            var totalCount = await taskItemQuery.CountAsync();

            var pagedTaskItems = await taskItemQuery.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

            return (pagedTaskItems, totalCount);
        }

        public async Task<TaskItem> GetByIdAsync(int id)
        {
            return await _dbcontext.TaskItems.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(TaskItem entity)
        {
            await _dbcontext.TaskItems.AddAsync(entity);
        }

        public async Task DeleteAsync(TaskItem entity)
        {
            _dbcontext.TaskItems.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }
    }
}
