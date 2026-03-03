namespace TaskFlow.Models
{
    public class Project
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;
        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}
