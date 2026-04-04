using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.DTOs.User;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Controllers
{

    [Authorize(Roles = "Admin")]
    [Route("api/[Controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilterRequest filter)
        {
            var result = await _service.GetPagedUsersAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _service.GetUserByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRequest dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.CreateUserAsync(dto, userId);
            return Created("", result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserRequest dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.UpdateUserAsync(id, dto, userId);
            return Ok(result);
        }
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelUser(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _service.CancelUserAsync(id, userId);
            return NoContent();
        }
    }
}
