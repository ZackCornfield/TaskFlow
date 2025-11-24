using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Dtos.Auth;
using TaskFlowApi.Services;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ITokenService tokenService)
        : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(request.Email)
                    || string.IsNullOrWhiteSpace(request.Password)
                )
                    return BadRequest("Email and password required.");

                var user = await authService.RegisterAsync(request);
                if (user is null)
                {
                    return Conflict("User already exists");
                }

                var token = tokenService.CreateToken(user);
                var result = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    Token = token,
                };
                return CreatedAtAction(nameof(Register), result);
            }
            catch (Exception e)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request."
                );
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(request.Email)
                    || string.IsNullOrWhiteSpace(request.Password)
                )
                    return BadRequest("Email and password required.");

                var user = await authService.LoginAsync(request);

                if (user is null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                var token = tokenService.CreateToken(user);
                var result = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    Token = token,
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request."
                );
            }
        }
    }
}
