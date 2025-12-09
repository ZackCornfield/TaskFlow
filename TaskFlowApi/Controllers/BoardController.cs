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
using TaskFlowApi.Services;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController(IBoardService boardService) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBoards()
        {
            try
            {
                var boards = await boardService.GetBoardsAsync();
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
                var board = await boardService.CreateBoardAsync(request);
                return CreatedAtAction(nameof(GetBoardById), new { id = board.Id }, board);
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
                if (
                    User.Identity?.Name == null
                    || !Guid.TryParse(User.Identity.Name, out var userId)
                )
                {
                    return BadRequest("Invalid user identity.");
                }

                var board = await boardService.GetBoardByIdAsync(id, userId);

                if (board is null)
                {
                    return NotFound($"Board with ID {id} not found or access denied.");
                }

                return Ok(board);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBoardById: {ex.Message}\n{ex.StackTrace}");
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
                var updatedBoard = await boardService.UpdateBoardAsync(id, request);
                if (updatedBoard is null)
                {
                    return NotFound($"Board with ID {id} not found.");
                }

                return Ok(updatedBoard);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while updating the board."
                );
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            try
            {
                var deleted = await boardService.DeleteBoardAsync(id);
                if (!deleted)
                {
                    return NotFound($"Board with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while deleting the board."
                );
            }
        }

        [HttpGet("users/{userId}/boards")]
        [Authorize]
        public async Task<IActionResult> GetUserBoards(Guid userId)
        {
            try
            {
                var boards = await boardService.GetUserBoardsAsync(userId);
                return Ok(boards);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving the user's boards."
                );
            }
        }
    }
}
