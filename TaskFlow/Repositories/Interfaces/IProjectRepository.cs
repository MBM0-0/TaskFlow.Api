using TaskFlow.Models;

namespace TaskFlow.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllAsync();

        Task<Project> GetByIdAsync(int id);

        Task AddAsync(Project entity);

        Task SaveChangesAsync();

    }
}
