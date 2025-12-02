using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Dtos.Task;
using TaskFlowApi.Services;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        [HttpPost("{columnId}")]
        [Authorize]
        public async Task<IActionResult> CreateTask(int columnId, [FromBody] TaskRequestDto request)
        {
            try
            {
                var result = await taskService.CreateTaskAsync(columnId, request);
                return CreatedAtAction(nameof(CreateTask), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the task."
                );
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskRequestDto request)
        {
            try
            {
                var result = await taskService.UpdateTaskAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while updating the task."
                );
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await taskService.DeleteTaskAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while deleting the task."
                );
            }
        }

        [HttpPatch("{id}/move")]
        [Authorize]
        public async Task<IActionResult> MoveTask(int id, [FromBody] MoveTaskDto request)
        {
            try
            {
                var result = await taskService.MoveTaskAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while moving the task."
                );
            }
        }
    }
}
