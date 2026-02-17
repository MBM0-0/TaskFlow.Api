namespace TaskFlow.DTOs.TaskItem
{
    public class UpdateTaskItemRequest
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }

        public int ProjectId { get; set; }
    }
}
