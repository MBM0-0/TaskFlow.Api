using TaskFlow.DTOs.Pagination;

namespace TaskFlow.DTOs.Project
{
    public class ProjectFilterRequest : PaginationQuery
    {
        public string? Name { get; set; }
        public int? CreatedByUserId { get; set; }
        public bool? DeletedAt { get; set; } = false;
    }
}
