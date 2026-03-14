using Mapster;
using TaskFlow.DTOs.Pagination;
using TaskFlow.DTOs.Project;
using TaskFlow.DTOs.TaskItem;
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

        public async Task<PagedResponse<ProjectListResponse>> GetPagedProjectsAsync(ProjectFilterRequest filter)
        {
            var (projects, TotalCount) = await _repository.GetPagedAsync(filter);

            return new PagedResponse<ProjectListResponse>
            {
                Items = projects.Adapt<List<ProjectListResponse>>(),
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = TotalCount
            };

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

        public async Task<ProjectDetailsResponse> CreateProjectAsync(ProjectRequest dto, int userId)
        {
            var entity = dto.Adapt<Project>();
            if(string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new BadRequestException("Project name is required.");
            }
            if (await _repository.IsNameFoundAsync(dto.Name))
            {
                throw new BadRequestException("A project with this name already exists.");
            }
            entity.CreatedByUserId = userId;

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity.Adapt<ProjectDetailsResponse>(); 
        }

        public async Task<ProjectDetailsResponse> UpdateProjectAsync(int id,ProjectRequest dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Project not found.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != entity.Name)
            {
                if (await _repository.IsNameFoundAsync(dto.Name))
                {
                    throw new BadRequestException("A project with this name already exists.");
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
