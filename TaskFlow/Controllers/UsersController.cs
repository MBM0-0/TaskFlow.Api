using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _service.GetAllUserAsync();
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
            var result = await _service.CreateUserAsync(dto);
            return Created("", result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserRequest dto)
        {
            var result = await _service.UpdateUserAsync(id, dto);
            return Ok(result);
        }
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelUser(int id)
        {
            await _service.CancelUserAsync(id);
            return NoContent();
        }
    }
}
