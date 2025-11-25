using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Board;
using TaskFlowApi.Dtos.Column;
using TaskFlowApi.Dtos.Task;
using TaskFlowApi.Models;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColumnController(TaskFlowDbContext dbContext) : ControllerBase
    {
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateColumn(int id, [FromBody] ColumnRequestDto request)
        {
            try
            {
                var column = await dbContext.Columns.FindAsync(id);
                if (column is null)
                {
                    return NotFound($"Column with ID {id} not found.");
                }

                column.Title = request.Title;
                column.SortOrder = request.SortOrder;

                await dbContext.SaveChangesAsync();

                return Ok(
                    new ColumnDto
                    {
                        Id = column.Id,
                        BoardId = column.BoardId,
                        Title = column.Title,
                        SortOrder = column.SortOrder,
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while updating the column."
                );
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(int id)
        {
            try
            {
                var column = await dbContext.Columns.FindAsync(id);
                if (column is null)
                {
                    return NotFound($"Column with ID {id} not found.");
                }

                dbContext.Columns.Remove(column);
                await dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while deleting the column."
                );
            }
        }

        [HttpPost("{columnId}/tasks")]
        public async Task<IActionResult> CreateTask(int columnId, [FromBody] TaskRequestDto request)
        {
            try
            {
                var column = await dbContext.Columns.FindAsync(columnId);
                if (column is null)
                {
                    return NotFound($"Column with ID {columnId} not found");
                }

                if (request.AssignedToId.HasValue)
                {
                    var userExists = await dbContext.Users.AnyAsync(u =>
                        u.Id == request.AssignedToId.Value
                    );
                    if (!userExists)
                    {
                        return BadRequest(
                            $"User with ID {request.AssignedToId.Value} does not exist."
                        );
                    }
                }

                var task = new TaskItem
                {
                    ColumnId = columnId,
                    Title = request.Title,
                    Description = request.Description,
                    SortOrder = request.SortOrder,
                    DueDate = request.DueDate,
                    CreatedById = request.CreatedById,
                    AssignedToId = request.AssignedToId,
                };

                dbContext.Tasks.Add(task);
                await dbContext.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(CreateTask),
                    new { id = task.Id },
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
                    "An error occurred while creating the task."
                );
            }
        }
    }
}
