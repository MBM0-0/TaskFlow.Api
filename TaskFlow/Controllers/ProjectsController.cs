using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.DTOs.Project;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectsController(IProjectService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] ProjectFilterRequest filter)
        {
            var result = await _service.GetPagedProjectsAsync(filter);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var result = await _service.GetProjectByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectRequest dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.CreateProjectAsync(dto, userId);
            return Created("", result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, ProjectRequest dto)
        {
            var result = await _service.UpdateProjectAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelProject(int id)
        {
            await _service.CancelProjectAsync(id);
            return NoContent();
        }
    }
}
