using Microsoft.AspNetCore.Mvc;
using LibraryOnlineRentalSystem.Domain.User;
using Microsoft.Extensions.Logging;

namespace LibraryOnlineRentalSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Auth([FromBody] AuthRequestDTO request)
    {
        try
        {
            _logger.LogInformation("Login attempt for user {Username} at {Time}", request.Username, DateTime.UtcNow);

            var response = await _authService.AuthAsync(request);
            _logger.LogInformation("Login successful for user {Username} at {Time}", request.Username, DateTime.UtcNow);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Login failed for user {Username} at {Time}: {Message}", request.Username, DateTime.UtcNow, ex.Message);

            return BadRequest(new { message = "Invalid username or password" });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDTO>> RefreshToken([FromBody] RefreshTokenRequestDTO request)
    {
        try
        {
            _logger.LogInformation("Refresh token requested at {Time}", DateTime.UtcNow);

            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Refresh token failed at {Time}: {Message}", DateTime.UtcNow, ex.Message);

            return BadRequest(new { message = "Invalid refresh token" });
        }
    }
}
