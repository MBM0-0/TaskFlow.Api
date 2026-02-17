using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs.Project;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectsController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var result = await _service.GetAllProjectAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var result = await _service.GetProjectByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectRequest dto)
        {
            var result = await _service.CreateProjectAsync(dto);
            return Created("", result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, ProjectRequest dto)
        {
            var result = await _service.UpdateProjectAsync(id, dto);
            return Ok(result);
        }
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelProject(int id)
        {
            await _service.CancelProjectAsync(id);
            return NoContent();
        }
    }
}
