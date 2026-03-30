using TaskFlow.Repositories;
using TaskFlow.Repositories.Interfaces;
using TaskFlow.Services;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddTaskFlowServices(this IServiceCollection services)
        {
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskItemRepository, TaskItemRepository>();
            services.AddScoped<ITaskItemService, TaskItemService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
