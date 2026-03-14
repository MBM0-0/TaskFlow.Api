using TaskFlow.DTOs.Pagination;
using TaskFlow.DTOs.Project;
using TaskFlow.Models;

namespace TaskFlow.Services.Interfaces
{
    public interface IProjectService
    {
        Task<PagedResponse<ProjectListResponse>> GetPagedProjectsAsync(ProjectFilterRequest filter);
        Task<ProjectDetailsResponse> GetProjectByIdAsync(int id);
        Task<ProjectDetailsResponse> CreateProjectAsync(ProjectRequest dto, int userId);
        Task<ProjectDetailsResponse> UpdateProjectAsync(int id, ProjectRequest dto);
        Task CancelProjectAsync(int id);

    }
}
