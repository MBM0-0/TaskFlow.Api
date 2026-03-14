using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs.User;
using TaskFlow.Models;
using TaskFlow.Repositories.Interfaces;

namespace TaskFlow.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbcontext;

        public UserRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<(List<User> Users, int TotalCount)> GetPagedAsync(UserFilterRequest filter)
        {
            IQueryable<User> userQuery = _dbcontext.Users;

            if (filter.DisabledAt.HasValue)
            {
                userQuery = filter.DisabledAt.Value
                  ? userQuery.Where(x => x.DisabledAt != null)
                  : userQuery.Where(x => x.DisabledAt == null);
            }

            if (!string.IsNullOrWhiteSpace(filter.UserName))
            {
                userQuery = userQuery.Where(x => x.UserName.Contains(filter.UserName));
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                userQuery = userQuery.Where(x => x.Email.Contains(filter.Email));
            }

            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                userQuery = userQuery.Where(x => x.FullName.Contains(filter.FullName));
            }

            userQuery = filter.SortDesc ? userQuery.OrderByDescending(x => x.CreatedAt) : userQuery.OrderBy(x => x.CreatedAt);

            var totalCount = await userQuery.CountAsync();

            var pagedUsers = await userQuery.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

            return (pagedUsers, totalCount);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _dbcontext.Users.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(User entity)
        {
            await _dbcontext.Users.AddAsync(entity);
        }

        public async Task<bool> IsUserNameFoundAsync(string UserName)
        {
            return await _dbcontext.Users.Where(x => x.DisabledAt == null).AnyAsync(x => x.UserName == UserName);
        }

        public async Task SaveChangesAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<bool> RoleExistsAsync(int roleId)
        {
            return await _dbcontext.Roles.AnyAsync(r => r.Id == roleId);
        }
    }
}

