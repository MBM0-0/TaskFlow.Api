using TaskFlow.DTOs.Pagination;

namespace TaskFlow.DTOs.User
{
    public class UserFilterRequest : PaginationQuery
    {
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? DisabledAt { get; set; } = false;
    }
}
