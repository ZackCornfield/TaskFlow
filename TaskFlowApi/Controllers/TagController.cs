using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Tag;
using TaskFlowApi.Models;
using TaskFlowApi.Services;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController(TaskFlowDbContext dbContext, ITagService tagService) : ControllerBase
    {
        // Create and Delete Tags
        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagRequestDto request)
        {
            try
            {
                var tag = await tagService.CreateTagAsync(request);

                return CreatedAtAction(nameof(CreateTag), new { id = tag.Id }, tag);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the tag."
                );
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                await tagService.DeleteTagAsync(id);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while deleting the tag."
                );
            }
        }

        [HttpPost("{taskId}/tags")]
        public async Task<IActionResult> AddTagsToTask(int taskId, [FromBody] List<int> tagIds)
        {
            try
            {
                await tagService.AddTagsToTaskAsync(taskId, tagIds);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{taskId}/tags")]
        public async Task<IActionResult> RemoveTagsFromTask(int taskId, [FromBody] List<int> tagIds)
        {
            try
            {
                await tagService.RemoveTagsFromTaskAsync(taskId, tagIds);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{taskId}/tags")]
        public async Task<IActionResult> GetTagsForTask(int taskId)
        {
            try
            {
                var tags = await tagService.GetTagsForTaskAsync(taskId);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
