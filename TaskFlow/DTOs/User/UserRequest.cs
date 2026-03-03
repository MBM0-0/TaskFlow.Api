namespace TaskFlow.DTOs.User
{
    public class UserRequest
    {
        public string? FullName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public int RoleId { get; set; }
    }
}
