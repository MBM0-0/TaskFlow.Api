using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;
using TaskFlow.Repositories.Interfaces;

namespace TaskFlow.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _dbcontext;

        public ProjectRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _dbcontext.Projects.Include(p => p.TaskItems.Where(t => t.DeletedAt == null)).Where(r => r.DeletedAt == null).ToListAsync();
        }

        public async Task<Project> GetByIdAsync(int id)
        {
            return await _dbcontext.Projects.Include(p => p.TaskItems).FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(Project entity)
        {
            await _dbcontext.Projects.AddAsync(entity);
        }

        public async Task<bool> IsNameFoundAsync(string name)
        {
            return await _dbcontext.Projects.Where(x => x.DeletedAt == null).AnyAsync(x => x.Name == name);
        }

        public async Task SaveChangesAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }
    }
}
