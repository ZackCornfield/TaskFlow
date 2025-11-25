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
        [HttpPost("{boardId}")]
        public async Task<IActionResult> CreateColumn(
            int boardId,
            [FromBody] ColumnRequestDto request
        )
        {
            try
            {
                var board = await dbContext.Boards.FindAsync(boardId);
                if (board is null)
                {
                    return NotFound($"Board with ID {boardId} not found.");
                }

                var column = new Column
                {
                    BoardId = boardId,
                    Title = request.Title,
                    SortOrder = request.SortOrder,
                };

                dbContext.Columns.Add(column);
                await dbContext.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(CreateColumn),
                    new { boardId = column.BoardId, id = column.Id },
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
                    "An error occurred while creating the column."
                );
            }
        }

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
    }
}
