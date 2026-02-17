using TaskFlow.DTOs.Project;

namespace TaskFlow.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectListResponse>> GetAllProjectAsync();
        Task<ProjectDetailsResponse> GetProjectByIdAsync(int id);
        Task<ProjectDetailsResponse> CreateProjectAsync(CreateProjectRequest dto);
        Task<ProjectDetailsResponse> UpdateProjectAsync(UpdateProjectRequest dto);
        Task CancelProjectAsync(int id);

    }
}
