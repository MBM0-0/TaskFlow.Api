using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs.TaskItem;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class TaskItemsController : Controller
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
        public async Task<IActionResult> CreateTaskItem(CreateTaskItemRequest dto)
        {
            var result = await _service.CreateTaskItemAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTaskItem(UpdateTaskItemRequest dto)
        {
            var result = await _service.UpdateTaskItemAsync(dto);
            return Ok(result);
        }
        [HttpPut("Cancel/{id}")]
        public async Task<IActionResult> CancelTaskItem(int id)
        {
            await _service.CancelTaskItemAsync(id);
            return Ok(new { Message = $"The Booking With Id {id} Has Been Cancelled" });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            await _service.DeleteTaskItemAsync(id);
            return Ok();
        }
    }
}
