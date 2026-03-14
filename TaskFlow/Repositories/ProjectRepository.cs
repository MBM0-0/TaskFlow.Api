using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs.Project;
using TaskFlow.Models;
using TaskFlow.Repositories.Interfaces;
using static System.Net.WebRequestMethods;

namespace TaskFlow.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _dbcontext;

        public ProjectRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<(List<Project> projects, int totalCount)> GetPagedAsync(ProjectFilterRequest filter)
        {
            IQueryable<Project> projectQuery = _dbcontext.Projects.Include(p => p.TaskItems);

            if (filter.DeletedAt.HasValue)
            {
                projectQuery = filter.DeletedAt.Value
                  ? projectQuery.Where(x => x.DeletedAt != null)
                  : projectQuery.Where(x => x.DeletedAt == null);
            }

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                projectQuery = projectQuery.Where(x => x.Name.Contains(filter.Name));
            }

            if (filter.CreatedByUserId.HasValue)
            {
                projectQuery = projectQuery.Where(x => x.CreatedByUserId == filter.CreatedByUserId);
            }

            projectQuery = filter.SortDesc ? projectQuery.OrderByDescending(x => x.CreatedAt) : projectQuery.OrderBy(x => x.CreatedAt);

            var totalCount = await projectQuery.CountAsync();

            var pagedProjects = await projectQuery.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

            return (pagedProjects, totalCount);
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
