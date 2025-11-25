using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
