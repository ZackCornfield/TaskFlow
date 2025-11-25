using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Board;
using TaskFlowApi.Dtos.Comment;
using TaskFlowApi.Dtos.Tag;
using TaskFlowApi.Dtos.Task;
using TaskFlowApi.Models;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(TaskFlowDbContext dbContext) : ControllerBase
    {
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskRequestDto request)
        {
            try
            {
                var task = await dbContext.Tasks.FindAsync(id);
                if (task is null)
                {
                    return NotFound($"Task with ID {id} not found.");
                }

                var newTask = new TaskItem
                {
                    Id = task.Id,
                    ColumnId = task.ColumnId,
                    Title = request.Title,
                    Description = request.Description,
                    SortOrder = request.SortOrder,
                    DueDate = request.DueDate,
                    CreatedById = request.CreatedById,
                    AssignedToId = request.AssignedToId,
                };

                dbContext.Entry(task).CurrentValues.SetValues(newTask);
                await dbContext.SaveChangesAsync();

                return Ok(
                    new TaskDto
                    {
                        Id = newTask.Id,
                        ColumnId = newTask.ColumnId,
                        Title = newTask.Title,
                        Description = newTask.Description,
                        SortOrder = newTask.SortOrder,
                        DueDate = newTask.DueDate,
                        CreatedById = newTask.CreatedById,
                        AssignedToId = newTask.AssignedToId,
                    }
                );
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
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await dbContext.Tasks.FindAsync(id);
                if (task is null)
                {
                    return NotFound($"Task with ID {id} not found.");
                }

                dbContext.Tasks.Remove(task);
                await dbContext.SaveChangesAsync();

                return NoContent();
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
        public async Task<IActionResult> MoveTask(int id, [FromBody] MoveTaskDto request)
        {
            try
            {
                var task = await dbContext.Tasks.FindAsync(id);
                if (task is null)
                {
                    return NotFound($"Task with ID {id} not found.");
                }

                var targetColumn = await dbContext.Columns.FindAsync(request.TargetColumnId);
                if (targetColumn is null)
                {
                    return NotFound($"Target column with ID {request.TargetColumnId} not found.");
                }

                task.ColumnId = request.TargetColumnId;
                task.SortOrder = request.SortOrder;

                await dbContext.SaveChangesAsync();

                return Ok(
                    new TaskDto
                    {
                        Id = task.Id,
                        ColumnId = task.ColumnId,
                        Title = task.Title,
                        Description = task.Description,
                        SortOrder = task.SortOrder,
                        DueDate = task.DueDate,
                        CreatedById = task.CreatedById,
                        AssignedToId = task.AssignedToId,
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while moving the task."
                );
            }
        }

        [HttpPost("{id}/tags")]
        public async Task<IActionResult> AddTagsToTask(int id, [FromBody] List<int> tagIds)
        {
            try
            {
                var task = await dbContext
                    .Tasks.Include(t => t.Tags)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (task is null)
                {
                    return NotFound($"Task with ID {id} not found.");
                }

                var tags = await dbContext.Tags.Where(tag => tagIds.Contains(tag.Id)).ToListAsync();
                if (tags.Count != tagIds.Count)
                {
                    return BadRequest("Some tags were not found.");
                }

                foreach (var tag in tags)
                {
                    if (!task.Tags.Contains(tag))
                    {
                        task.Tags.Add(tag);
                    }
                }

                await dbContext.SaveChangesAsync();

                return Ok(
                    task.Tags.Select(tag => new TagDto
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        Color = tag.Color,
                    })
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while adding tags to the task."
                );
            }
        }

        [HttpDelete("{id}/tags")]
        public async Task<IActionResult> RemoveTagsFromTask(int id, [FromBody] List<int> tagIds)
        {
            try
            {
                var task = await dbContext
                    .Tasks.Include(t => t.Tags)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (task is null)
                {
                    return NotFound($"Task with ID {id} not found.");
                }

                task.Tags = task.Tags.Where(tag => !tagIds.Contains(tag.Id)).ToList();

                await dbContext.SaveChangesAsync();

                return Ok(
                    task.Tags.Select(tag => new TagDto
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        Color = tag.Color,
                    })
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while removing tags from the task."
                );
            }
        }

        [HttpGet("{id}/tags")]
        public async Task<IActionResult> GetTagsForTask(int id)
        {
            try
            {
                var task = await dbContext
                    .Tasks.Include(t => t.Tags)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (task is null)
                {
                    return NotFound($"Task with ID {id} not found.");
                }

                return Ok(
                    task.Tags.Select(tag => new TagDto
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        Color = tag.Color,
                    })
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving tags for the task."
                );
            }
        }

        [HttpPost("{taskId}/comments")]
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

                dbContext.Comments.Add(comment);
                await dbContext.SaveChangesAsync();

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

        [HttpPut("{taskId}/comments/{commentId}")]
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

        [HttpDelete("{taskId}/comments/{commentId}")]
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

        [HttpGet("{taskId}/comments")]
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
