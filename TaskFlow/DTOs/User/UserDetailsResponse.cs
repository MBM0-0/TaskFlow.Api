namespace TaskFlow.DTOs.User
{
    public class UserDetailsResponse
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DisabledAt { get; set; }
    }
}
