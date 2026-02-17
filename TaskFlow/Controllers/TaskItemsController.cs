using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> GetAllTaskItem()
        {
            var result = await _service.GetAllTaskItemAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskItemById(int id)
        {
            var result = await _service.GetTaskItemByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaskItem(TaskItemRequest dto)
        {
            var result = await _service.CreateTaskItemAsync(dto);
            return Created("", result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskItem(int id, TaskItemRequest dto)
        {
            var result = await _service.UpdateTaskItemAsync(id, dto);
            return Ok(result);
        }
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelTaskItem(int id)
        {
            await _service.CancelTaskItemAsync(id);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            await _service.DeleteTaskItemAsync(id);
            return NoContent();
        }
    }
}
