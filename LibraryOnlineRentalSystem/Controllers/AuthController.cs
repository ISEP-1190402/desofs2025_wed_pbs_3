using Microsoft.AspNetCore.Mvc;
using LibraryOnlineRentalSystem.Domain.User;

namespace LibraryOnlineRentalSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Auth([FromBody] AuthRequestDTO request)
    {
        try
        {
            var response = await _authService.AuthAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Invalid username or password" });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDTO>> RefreshToken([FromBody] RefreshTokenRequestDTO request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Invalid refresh token" });
        }
    }
}
