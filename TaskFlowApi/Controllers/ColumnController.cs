using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Dtos.Column;
using TaskFlowApi.Services;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColumnController(IColumnService columnService) : ControllerBase
    {
        [HttpPost("{boardId}")]
        [Authorize]
        public async Task<IActionResult> CreateColumn(
            int boardId,
            [FromBody] ColumnRequestDto request
        )
        {
            try
            {
                var result = await columnService.CreateColumnAsync(boardId, request);
                return CreatedAtAction(
                    nameof(CreateColumn),
                    new { boardId = result.BoardId, id = result.Id },
                    result
                );
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
        [Authorize]
        public async Task<IActionResult> UpdateColumn(int id, [FromBody] ColumnRequestDto request)
        {
            try
            {
                var result = await columnService.UpdateColumnAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
        [Authorize]
        public async Task<IActionResult> DeleteColumn(int id)
        {
            try
            {
                await columnService.DeleteColumnAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
