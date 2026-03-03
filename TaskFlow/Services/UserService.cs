using Mapster;
using TaskFlow.DTOs.Project;
using TaskFlow.DTOs.User;
using TaskFlow.Middlewares.Exceptions;
using TaskFlow.Models;
using TaskFlow.Repositories.Interfaces;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UserListResponse>> GetAllUserAsync()
        {
            var entity = await _repository.GetAllAsync();
            return entity.Adapt<List<UserListResponse>>();
        }

        public async Task<UserDetailsResponse> GetUserByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("User not found.");
            }
            return entity.Adapt<UserDetailsResponse>();
        }

        public async Task<UserDetailsResponse> CreateUserAsync(UserRequest dto)
        {
            var entity = dto.Adapt<User>();
            if (string.IsNullOrWhiteSpace(dto.UserName))
            {
                throw new BadRequestException("UserName is required.");
            }
            if (await _repository.IsUserNameFoundAsync(dto.UserName))
            {
                throw new BadRequestException("A User with this name already exists.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new BadRequestException("Email is required.");
            }
            if (string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                throw new BadRequestException("Password is required.");
            }

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity.Adapt<UserDetailsResponse>();
        }

        public async Task<UserDetailsResponse> UpdateUserAsync(int id, UserRequest dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.DisabledAt != null)
            {
                throw new NotFoundException("User not found or has been Disabled.");
            }

            if (!string.IsNullOrWhiteSpace(dto.UserName) && dto.UserName != entity.UserName)
            {
                if (await _repository.IsUserNameFoundAsync(dto.UserName))
                {
                    throw new BadRequestException("A user with this name already exists.");
                }
                entity.UserName = dto.UserName;
            }

            entity.FullName = dto.FullName;
            entity.Email = dto.Email;
            entity.PasswordHash = dto.PasswordHash;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return entity.Adapt<UserDetailsResponse>();
        }

        public async Task CancelUserAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.DisabledAt != null)
            {
                throw new NotFoundException("User not found or already canceled.");
            }

            entity.DisabledAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
        }
    }
}
