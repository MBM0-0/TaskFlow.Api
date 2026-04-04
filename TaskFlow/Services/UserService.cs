using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.DTOs.Pagination;
using TaskFlow.DTOs.Project;
using TaskFlow.DTOs.TaskItem;
using TaskFlow.DTOs.User;
using TaskFlow.Extensions.Middlewares.Exceptions;
using TaskFlow.Models;
using TaskFlow.Repositories.Interfaces;
using TaskFlow.Services.Interfaces;
using static System.Net.WebRequestMethods;

namespace TaskFlow.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly PasswordHasher<User> _passwordHasher = new();
        private readonly AuditService _auditService;

        public UserService(IUserRepository repository, AuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<PagedResponse<UserListResponse>> GetPagedUsersAsync(UserFilterRequest filter)
        {
            var (users, totalCount) = await _repository.GetPagedAsync(filter);

            return new PagedResponse<UserListResponse>
            {
                Items = users.Adapt<List<UserListResponse>>(),
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
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

        public async Task<UserDetailsResponse> CreateUserAsync(UserRequest dto, int userId)
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

            if (!await _repository.RoleExistsAsync(dto.RoleId))
            {
                throw new BadRequestException($"RoleId {dto.RoleId} does not exist.");
            }

            entity.PasswordHash = _passwordHasher.HashPassword(entity, dto.PasswordHash);

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            await _auditService.LogAsync(userId, "Create", "User", entity.Id);
            await _repository.SaveChangesAsync();

            return entity.Adapt<UserDetailsResponse>();
        }

        public async Task<UserDetailsResponse> UpdateUserAsync(int id, UserRequest dto, int userId)
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
            if (!await _repository.RoleExistsAsync(dto.RoleId))
            {
                throw new BadRequestException($"RoleId {dto.RoleId} does not exist.");
            }

            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                entity.PasswordHash = _passwordHasher.HashPassword(entity, dto.PasswordHash);
            }

            entity.FullName = dto.FullName;
            entity.Email = dto.Email;
            entity.UpdatedAt = DateTime.UtcNow;
            await _auditService.LogAsync(userId, "Update", "User", id);
            await _repository.SaveChangesAsync();
            return entity.Adapt<UserDetailsResponse>();
        }

        public async Task CancelUserAsync(int id, int userId)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.DisabledAt != null)
            {
                throw new NotFoundException("User not found or already canceled.");
            }

            entity.DisabledAt = DateTime.UtcNow;
            await _auditService.LogAsync(userId, "Cancel", "User", id);
            await _repository.SaveChangesAsync();
        }
    }
}
