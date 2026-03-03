using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.DTOs.TaskItem;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class TaskItemsController : ControllerBase
    {
        private readonly ITaskItemService _service;

        public TaskItemsController(ITaskItemService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAllTaskItem()
        {
            var result = await _service.GetAllTaskItemAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskItemById(int id)
        {
            var result = await _service.GetTaskItemByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTaskItem(TaskItemRequest dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.CreateTaskItemAsync(dto, userId);
            return Created("", result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskItem(int id, TaskItemRequest dto)
        {
            var result = await _service.UpdateTaskItemAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelTaskItem(int id)
        {
            await _service.CancelTaskItemAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            await _service.DeleteTaskItemAsync(id);
            return NoContent();
        }
    }
}
