using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Dtos.Board;
using TaskFlowApi.Services;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardMemberController(IBoardMemberService boardMemberService) : ControllerBase
    {
        [HttpPost("{boardId}/members")]
        public async Task<IActionResult> AddBoardMember(
            int boardId,
            [FromBody] AddBoardMemberDto request
        )
        {
            try
            {
                var boardMember = await boardMemberService.AddBoardMemberAsync(boardId, request);
                if (boardMember is null)
                {
                    return NotFound("Board or user not found.");
                }

                return CreatedAtAction(
                    nameof(AddBoardMember),
                    new { boardId, userId = request.UserId },
                    boardMember
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while adding the board member."
                );
            }
        }

        [HttpDelete("{boardId}/members/{userId}")]
        public async Task<IActionResult> RemoveBoardMember(int boardId, Guid userId)
        {
            try
            {
                var removed = await boardMemberService.RemoveBoardMemberAsync(boardId, userId);
                if (!removed)
                {
                    return NotFound("Board member not found.");
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while removing the board member."
                );
            }
        }

        [HttpGet("{boardId}/members")]
        public async Task<IActionResult> GetBoardMembers(int boardId)
        {
            try
            {
                var members = await boardMemberService.GetBoardMembersAsync(boardId);
                if (members is null)
                {
                    return NotFound("Board not found.");
                }

                return Ok(members);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving board members."
                );
            }
        }
    }
}
