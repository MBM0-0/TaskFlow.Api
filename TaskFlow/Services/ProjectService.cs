using Mapster;
using TaskFlow.DTOs.Project;
using TaskFlow.Middlewares.Exceptions;
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
            if (entity == null)
            {
                throw new NotFoundException("Project not found.");
            }
            return entity.Adapt<ProjectDetailsResponse>();
        }

        public async Task<ProjectDetailsResponse> CreateProjectAsync(ProjectRequest dto)
        {
            var entity = dto.Adapt<Project>();
            if(string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ValidationException("Project name is required.");
            }
            if (await _repository.IsNameFoundAsync(dto.Name))
            {
                throw new ValidationException("A project with this name already exists.");
            }

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity.Adapt<ProjectDetailsResponse>(); 
        }

        public async Task<ProjectDetailsResponse> UpdateProjectAsync(int id,ProjectRequest dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.DeletedAt != null)
            {
                throw new NotFoundException("Project not foundor has been deleted..");
            }

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != entity.Name)
            {
                if (await _repository.IsNameFoundAsync(dto.Name))
                {
                    throw new ValidationException("A project with this name already exists.");
                }
                entity.Name = dto.Name;
            }

            entity.Description = dto.Description;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return entity.Adapt<ProjectDetailsResponse>();
        }

        public async Task CancelProjectAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.DeletedAt != null)
            {
                throw new NotFoundException("Project not found or already canceled.");
            }

            entity.DeletedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
        }
    }
}
