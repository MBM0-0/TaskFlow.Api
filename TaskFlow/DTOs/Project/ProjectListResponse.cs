using TaskFlow.DTOs.TaskItem;

namespace TaskFlow.DTOs.Project
{
    public class ProjectListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<TaskItemListResponse> TaskItems { get; set; }
    }
}
