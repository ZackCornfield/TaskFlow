using Microsoft.AspNetCore.Authorization;
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
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTags()
        {
            try
            {
                var tags = await tagService.GetAllTagsAsync();
                if (tags == null || tags.Count == 0)
                {
                    return NotFound("No tags found.");
                }
                return Ok(tags);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving tags."
                );
            }
        }

        // Create and Delete Tags
        [HttpPost]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> AddTagsToTask(int taskId, [FromBody] List<int> tagIds)
        {
            try
            {
                var tags = await tagService.AddTagsToTaskAsync(taskId, tagIds);
                return Ok(tags);
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
        [Authorize]
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
        [Authorize]
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
