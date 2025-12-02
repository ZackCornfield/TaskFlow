using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Comment;
using TaskFlowApi.Models;
using TaskFlowApi.Services;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController(ICommentService commentService) : ControllerBase
    {
        [HttpPost("{taskId}")]
        [Authorize]
        public async Task<IActionResult> CreateComment(
            int taskId,
            [FromBody] CommentRequestDto request
        )
        {
            try
            {
                var comment = await commentService.CreateCommentAsync(taskId, request);

                return CreatedAtAction(
                    nameof(GetCommentsForTask),
                    new { taskId },
                    new CommentDto
                    {
                        Id = comment.Id,
                        TaskId = comment.TaskId,
                        AuthorId = comment.AuthorId,
                        Content = comment.Content,
                        CreatedAt = comment.CreatedAt,
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{taskId}/{commentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(
            int taskId,
            int commentId,
            [FromBody] CommentRequestDto request
        )
        {
            try
            {
                var comment = await commentService.UpdateCommentAsync(commentId, request);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{taskId}/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int taskId, int commentId)
        {
            try
            {
                await commentService.DeleteCommentAsync(commentId);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{taskId}")]
        [Authorize]
        public async Task<IActionResult> GetCommentsForTask(int taskId)
        {
            try
            {
                var comments = await commentService.GetCommentsForTaskAsync(taskId);

                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
