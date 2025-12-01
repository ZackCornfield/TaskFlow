using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Auth;
using TaskFlowApi.Dtos.Board;
using TaskFlowApi.Services;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly ITokenService tokenService;
        private readonly TaskFlowDbContext dbContext;

        public AuthController(
            IAuthService authService,
            ITokenService tokenService,
            TaskFlowDbContext dbContext
        )
        {
            this.authService = authService;
            this.tokenService = tokenService;
            this.dbContext = dbContext;
        }

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
                    CreatedAt = user.CreatedAt,
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
                    CreatedAt = user.CreatedAt,
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
