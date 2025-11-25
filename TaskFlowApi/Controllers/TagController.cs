using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Tag;
using TaskFlowApi.Models;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController(TaskFlowDbContext dbContext) : ControllerBase
    {
        // Create and Delete Tags
        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagRequestDto request)
        {
            try
            {
                var tag = new Tag { Name = request.Name, Color = request.Color };

                // Assuming dbContext is available via dependency injection
                dbContext.Tags.Add(tag);
                await dbContext.SaveChangesAsync();

                var tagDto = new TagDto
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Color = tag.Color,
                };

                return CreatedAtAction(nameof(CreateTag), new { id = tag.Id }, tagDto);
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
                var tag = await dbContext.Tags.FindAsync(id);
                if (tag is null)
                {
                    return NotFound($"Tag with ID {id} not found.");
                }

                dbContext.Tags.Remove(tag);
                await dbContext.SaveChangesAsync();

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
                var task = await dbContext
                    .Tasks.Include(t => t.Tags)
                    .FirstOrDefaultAsync(t => t.Id == taskId);
                if (task is null)
                {
                    return NotFound($"Task with ID {taskId} not found.");
                }

                var tags = await dbContext.Tags.Where(tag => tagIds.Contains(tag.Id)).ToListAsync();
                if (tags.Count != tagIds.Count)
                {
                    return BadRequest("Some tags do not exist.");
                }

                foreach (var tag in tags)
                {
                    if (!task.Tags.Contains(tag))
                    {
                        task.Tags.Add(tag);
                    }
                }

                await dbContext.SaveChangesAsync();

                return NoContent();
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
                var task = await dbContext
                    .Tasks.Include(t => t.Tags)
                    .FirstOrDefaultAsync(t => t.Id == taskId);
                if (task is null)
                {
                    return NotFound($"Task with ID {taskId} not found.");
                }

                var tagsToRemove = task.Tags.Where(tag => tagIds.Contains(tag.Id)).ToList();
                foreach (var tag in tagsToRemove)
                {
                    task.Tags.Remove(tag);
                }

                await dbContext.SaveChangesAsync();

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
                var task = await dbContext
                    .Tasks.Include(t => t.Tags)
                    .FirstOrDefaultAsync(t => t.Id == taskId);
                if (task is null)
                {
                    return NotFound($"Task with ID {taskId} not found.");
                }

                var tags = task
                    .Tags.Select(tag => new TagDto
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        Color = tag.Color,
                    })
                    .ToList();

                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
