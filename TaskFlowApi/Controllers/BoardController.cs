using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Board;
using TaskFlowApi.Dtos.Column;
using TaskFlowApi.Dtos.Comment;
using TaskFlowApi.Dtos.Tag;
using TaskFlowApi.Models;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController(TaskFlowDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBoards()
        {
            try
            {
                var boards = await dbContext
                    .Boards.Select(b => new BoardDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        OwnerId = b.OwnerId,
                        CreatedAt = b.CreatedAt,
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(boards);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving boards."
                );
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBoard([FromBody] BoardRequestDto request)
        {
            try
            {
                var board = new Board
                {
                    Title = request.Title,
                    OwnerId = request.OwnerId,
                    CreatedAt = request.CreatedAt,
                };

                dbContext.Boards.Add(board);
                await dbContext.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetBoardById),
                    new { id = board.Id },
                    new BoardDto
                    {
                        Id = board.Id,
                        Title = board.Title,
                        OwnerId = board.OwnerId,
                        CreatedAt = board.CreatedAt,
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the board."
                );
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBoardById(int id)
        {
            try
            {
                var board = await dbContext
                    .Boards.Include(b => b.Columns)
                        .ThenInclude(c => c.Tasks)
                            .ThenInclude(t => t.Tags)
                    .Include(b => b.Columns)
                        .ThenInclude(c => c.Tasks)
                            .ThenInclude(t => t.Comments)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (board is null)
                {
                    return NotFound($"Board with ID {id} not found.");
                }

                return Ok(
                    new BoardDto
                    {
                        Id = board.Id,
                        Title = board.Title,
                        OwnerId = board.OwnerId,
                        CreatedAt = board.CreatedAt,
                        Columns = board
                            .Columns.Select(c => new ColumnDto
                            {
                                Id = c.Id,
                                BoardId = c.BoardId,
                                Title = c.Title,
                                SortOrder = c.SortOrder,
                                Tasks = c
                                    .Tasks.Select(t => new TaskDto
                                    {
                                        Id = t.Id,
                                        ColumnId = t.ColumnId,
                                        Title = t.Title,
                                        Description = t.Description,
                                        SortOrder = t.SortOrder,
                                        DueDate = t.DueDate,
                                        CreatedById = t.CreatedById,
                                        AssignedToId = t.AssignedToId,
                                        Tags = t
                                            .Tags.Select(tag => new TagDto
                                            {
                                                Id = tag.Id,
                                                Name = tag.Name,
                                                Color = tag.Color,
                                            })
                                            .ToList(),
                                        Comments = t
                                            .Comments.Select(comment => new CommentDto
                                            {
                                                Id = comment.Id,
                                                TaskId = comment.TaskId,
                                                AuthorId = comment.AuthorId,
                                                Content = comment.Content,
                                                CreatedAt = comment.CreatedAt,
                                            })
                                            .ToList(),
                                    })
                                    .ToList(),
                            })
                            .ToList(),
                        Tasks = board
                            .Columns.SelectMany(c => c.Tasks)
                            .Select(t => new TaskDto
                            {
                                Id = t.Id,
                                ColumnId = t.ColumnId,
                                Title = t.Title,
                                Description = t.Description,
                                SortOrder = t.SortOrder,
                                DueDate = t.DueDate,
                                CreatedById = t.CreatedById,
                                AssignedToId = t.AssignedToId,
                                Tags = t
                                    .Tags.Select(tag => new TagDto
                                    {
                                        Id = tag.Id,
                                        Name = tag.Name,
                                        Color = tag.Color,
                                    })
                                    .ToList(),
                                Comments = t
                                    .Comments.Select(comment => new CommentDto
                                    {
                                        Id = comment.Id,
                                        TaskId = comment.TaskId,
                                        AuthorId = comment.AuthorId,
                                        Content = comment.Content,
                                        CreatedAt = comment.CreatedAt,
                                    })
                                    .ToList(),
                            })
                            .ToList(),
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving the board."
                );
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBoard(int id, [FromBody] BoardRequestDto request)
        {
            try
            {
                var board = await dbContext.Boards.FindAsync(id);
                if (board is null)
                {
                    return NotFound($"Board with ID {id} not found.");
                }

                var updatedBoard = new Board
                {
                    Id = board.Id,
                    Title = request.Title,
                    OwnerId = request.OwnerId,
                    CreatedAt = request.CreatedAt,
                };

                dbContext.Entry(board).CurrentValues.SetValues(updatedBoard);
                await dbContext.SaveChangesAsync();
                return Ok(
                    new BoardDto
                    {
                        Id = updatedBoard.Id,
                        Title = updatedBoard.Title,
                        OwnerId = updatedBoard.OwnerId,
                        CreatedAt = updatedBoard.CreatedAt,
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the board."
                );
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            try
            {
                var board = await dbContext.Boards.FindAsync(id);
                if (board is null)
                {
                    return NotFound($"Board with ID {id} not found.");
                }

                dbContext.Boards.Remove(board);
                await dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the board."
                );
            }
        }
    }
}
