using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class AuditService
    {
        private readonly AppDbContext _dbcontext;

        public AuditService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task LogAsync(int userId, string action, string entity, int entityId)
        {
            _dbcontext.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                Entity = entity,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
