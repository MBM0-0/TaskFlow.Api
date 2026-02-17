using Mapster;
using TaskFlow.DTOs.Project;
using TaskFlow.Models;
using TaskFlow.Repositories.Interfaces;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repository;

        public ProjectService(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProjectListResponse>> GetAllProjectAsync()
        {
            var entity = await _repository.GetAllAsync();
            return entity.Adapt<List<ProjectListResponse>>();
        }

        public async Task<ProjectDetailsResponse> GetProjectByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity.Adapt<ProjectDetailsResponse>();
        }

        public async Task<ProjectDetailsResponse> CreateProjectAsync(CreateProjectRequest dto)
        {
            var entity = dto.Adapt<Project>();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity.Adapt<ProjectDetailsResponse>();
        }

        public async Task<ProjectDetailsResponse> UpdateProjectAsync(UpdateProjectRequest dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            entity.UpdatedAt = DateTime.UtcNow;

            entity.Id = dto.Id;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return entity.Adapt<ProjectDetailsResponse>();
        }

        public async Task CancelProjectAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            entity.DeletedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
        }
    }
}
