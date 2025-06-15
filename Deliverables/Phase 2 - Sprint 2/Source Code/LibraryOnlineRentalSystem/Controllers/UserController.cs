using Microsoft.AspNetCore.Authorization;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace LibraryOnlineRentalSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(UserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        _logger.LogInformation("User lookup requested for ID {Id} at {Time}", id, DateTime.UtcNow);
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User with ID {Id} not found at {Time}", id, DateTime.UtcNow);
            return NotFound();
        }

        _logger.LogInformation("User with ID {Id} returned successfully at {Time}", id, DateTime.UtcNow);
        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<IActionResult> GetAllUsers()
    {
        _logger.LogInformation("All users fetch initiated at {Time}", DateTime.UtcNow);
        var users = await _userService.GetAllUsersAsync();
        _logger.LogInformation("Returned {Count} users at {Time}", users.Count, DateTime.UtcNow);
        return Ok(users);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] NewUserDTO request)
    {
        _logger.LogInformation("New user registration attempt for email {Email} at {Time}", request.Email, DateTime.UtcNow);
        try
        {
            await _userService.CreateUserAsync(request);
            _logger.LogInformation("User {Email} registered successfully at {Time}", request.Email, DateTime.UtcNow);
            return Ok(new { message = "User registered successfully" });
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning("User registration failed for {Email}: {Message} at {Time}", request.Email, ex.Message, DateTime.UtcNow);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("User update attempt by {CurrentUserId} for user ID {TargetId} at {Time}", currentUserId, id, DateTime.UtcNow);

        if (User.IsInRole("Admin") || currentUserId == id.ToString())
        {
            await _userService.UpdateUserAsync(id, request);
            _logger.LogInformation("User ID {Id} updated successfully by {CurrentUserId} at {Time}", id, currentUserId, DateTime.UtcNow);
            return Ok("User updated.");
        }

        _logger.LogWarning("Unauthorized update attempt by {CurrentUserId} for user ID {TargetId} at {Time}", currentUserId, id, DateTime.UtcNow);
        return Forbid();
    }

    public class UpdateUserRequest
    {
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Biography { get; set; }
        
        public string? Nif { get; set; }
    }
}
