using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;
using TaskFlow.Repositories.Interfaces;

namespace TaskFlow.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly AppDbContext _dbcontext;

        public TaskItemRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _dbcontext.TaskItems.Where(r => r.DeletedAt == null).ToListAsync();
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
