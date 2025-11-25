using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Comment;
using TaskFlowApi.Models;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController(TaskFlowDbContext dbContext) : ControllerBase
    {
        [HttpPost("{taskId}")]
        public async Task<IActionResult> CreateComment(
            int taskId,
            [FromBody] CommentRequestDto request
        )
        {
            try
            {
                var task = await dbContext
                    .Tasks.Include(t => t.Comments)
                    .FirstOrDefaultAsync(t => t.Id == taskId);
                if (task is null)
                {
                    return NotFound($"Task with ID {taskId} not found.");
                }

                var comment = new Comment
                {
                    TaskId = taskId,
                    AuthorId = request.AuthorId,
                    Content = request.Content,
                    CreatedAt = request.CreatedAt,
                };

                task.Comments.Add(comment);
                await dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCommentsForTask), new { taskId }, comment);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{taskId}/{commentId}")]
        public async Task<IActionResult> UpdateComment(
            int taskId,
            int commentId,
            [FromBody] CommentRequestDto request
        )
        {
            try
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c =>
                    c.Id == commentId && c.TaskId == taskId
                );
                if (comment is null)
                {
                    return NotFound($"Comment with ID {commentId} not found for Task {taskId}.");
                }

                comment.Content = request.Content;
                comment.CreatedAt = request.CreatedAt;

                await dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{taskId}/{commentId}")]
        public async Task<IActionResult> DeleteComment(int taskId, int commentId)
        {
            try
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c =>
                    c.Id == commentId && c.TaskId == taskId
                );
                if (comment is null)
                {
                    return NotFound($"Comment with ID {commentId} not found for Task {taskId}.");
                }

                dbContext.Comments.Remove(comment);
                await dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetCommentsForTask(int taskId)
        {
            try
            {
                var task = await dbContext
                    .Tasks.Include(t => t.Comments)
                    .FirstOrDefaultAsync(t => t.Id == taskId);
                if (task is null)
                {
                    return NotFound($"Task with ID {taskId} not found.");
                }

                var comments = task
                    .Comments.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        TaskId = c.TaskId,
                        AuthorId = c.AuthorId,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                    })
                    .ToList();

                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
