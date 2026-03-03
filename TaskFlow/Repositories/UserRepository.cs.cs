using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
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

        public async Task<List<User>> GetAllAsync()
        {
            return await _dbcontext.Users.OrderBy(n =>n.FullName).ThenBy(n=>n.CreatedAt).Where(d => d.DisabledAt == null).ToListAsync();
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
    }
}

