using TaskFlow.DTOs.Pagination;

namespace TaskFlow.DTOs.TaskItem
{
    public class TaskItemFilterRequest : PaginationQuery
    {
        public TaskStatus? Status { get; set; }
        public string? Title { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }
        public bool? DeletedAt { get; set; } = false;
    }
}
