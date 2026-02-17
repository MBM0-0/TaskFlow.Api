namespace TaskFlow.DTOs.TaskItem
{
    public class CreateTaskItemRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }

        public int ProjectId { get; set; }
    }
}
