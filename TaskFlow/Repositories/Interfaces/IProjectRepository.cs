using TaskFlow.Models;

namespace TaskFlow.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllAsync();

        Task<Project> GetByIdAsync(int id);

        Task AddAsync(Project entity);

        Task<bool> IsNameFoundAsync(string name);

        Task SaveChangesAsync();

    }
}
