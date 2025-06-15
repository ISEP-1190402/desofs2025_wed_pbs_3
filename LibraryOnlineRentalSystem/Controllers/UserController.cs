using Microsoft.AspNetCore.Authorization;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

    // GET: api/user/id/{id} working
    // Access: Admin, LibraryManager
    [HttpGet("id/{id}")]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        _logger.LogInformation("=== GetUserById Start ===");
        _logger.LogInformation("User lookup by ID {Id} requested at {Time}", id, DateTime.UtcNow);
        
        var currentUser = await GetCurrentUserInfo();
        _logger.LogDebug("Current user - Username: {Username}, UserId: {UserId}, Roles: {Roles}", 
            currentUser.Username, 
            currentUser.UserId,
            string.Join(", ", currentUser.Roles));

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User with ID {Id} not found at {Time}", id, DateTime.UtcNow);
            return NotFound(new { message = "User not found" });
        }

        _logger.LogInformation("User with ID {Id} returned successfully to {Requester} at {Time}", 
            id, currentUser.Username, DateTime.UtcNow);
        return Ok(user);
    }

    // GET: api/user/username/{username} working
    // Access: Any authenticated user (own profile), Admin/LibraryManager (any profile)
    [HttpGet("username/{username}")]
    [Authorize]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        _logger.LogInformation("=== GetUserByUsername Start ===");
        _logger.LogInformation("Requested username: {RequestedUsername}", username);
        
        // Using preferred_username from token (mapped in Startup.cs)
        var currentUsername = User.Identity?.Name;
        _logger.LogDebug("Current username from token: {TokenUsername}", currentUsername);
        _logger.LogDebug("Current user roles: {Roles}", 
            string.Join(", ", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));
            
        if (string.IsNullOrEmpty(currentUsername))
        {
            _logger.LogWarning("Username (preferred_username) claim is missing in the token");
            return Unauthorized(new { message = "Invalid user identity: Username claim missing" });
        }

        var isAdmin = User.IsInRole("Admin") || User.IsInRole("LibraryManager");

        // Users can only see their own profile unless they're admins
        if (!isAdmin && !string.Equals(currentUsername, username, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Unauthorized profile access attempt by {User} for {Username}", 
                currentUsername, username);
            return StatusCode(403, new { message = "You can only view your own profile" });
        }

        _logger.LogInformation("User lookup by username {Username} requested by {User} at {Time}", 
            username, currentUsername, DateTime.UtcNow);
            
        var userProfile = await _userService.GetUserByUsernameAsync(username);
        if (userProfile == null)
        {
            _logger.LogWarning("User with username {Username} not found at {Time}", 
                username, DateTime.UtcNow);
            return NotFound(new { message = "User not found" });
        }

        _logger.LogInformation("User {Username} returned successfully at {Time}", 
            username, DateTime.UtcNow);
        return Ok(userProfile);
    }

    // GET: api/user working 
    // Access: Admin, LibraryManager
    [HttpGet]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<IActionResult> GetAllUsers()
    {
        _logger.LogInformation("=== GetAllUsers Start ===");
        var currentUser = await GetCurrentUserInfo();
        
        _logger.LogInformation("User list requested by {Username} at {Time}", 
            currentUser.Username, DateTime.UtcNow);
            
        var users = await _userService.GetAllUsersAsync();
        
        _logger.LogInformation("Returned {Count} users to {Requester} at {Time}", 
            users.Count, currentUser.Username, DateTime.UtcNow);
            
        return Ok(users);
    }

    // POST: api/user/register working
    // Access: Public
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] NewUserDTO request)
    {
        _logger.LogInformation("=== Register User Start ===");
        _logger.LogInformation("New user registration attempt for email {Email}", request.Email);
        
        try
        {
            await _userService.CreateUserAsync(request);
            _logger.LogInformation("User {Email} registered successfully at {Time}", 
                request.Email, DateTime.UtcNow);
                
            return Ok(new { message = "User registered successfully" });
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning("User registration failed for {Email}: {Message}", 
                request.Email, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    // PUT: api/user/profile/{username} working
    // Access: Any authenticated user (own profile only)
    [HttpPut("profile/{username}")]
    [Authorize]
    public async Task<IActionResult> UpdateMyProfile(string username, [FromBody] UpdateUserRequest request)
    {
        _logger.LogInformation("=== UpdateMyProfile Start ===");
        _logger.LogInformation("Updating profile for username: {Username}", username);
        _logger.LogDebug("Request data: {@RequestData}", request);
        
        // Using preferred_username from token (mapped in Startup.cs)
        var tokenUsername = User.Identity?.Name;
        _logger.LogDebug("Current user from token - Username: {TokenUsername}, UserId: {UserId}", 
            tokenUsername, 
            User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (string.IsNullOrEmpty(tokenUsername))
        {
            _logger.LogWarning("Username (preferred_username) not found in token claims");
            return Unauthorized("Username not found in token");
        }

        // Verify the username in the token matches the requested username
        if (!string.Equals(tokenUsername, username, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Unauthorized: Token username {TokenUsername} does not match requested username {Username}", 
                tokenUsername, username);
            return Forbid("You can only update your own profile");
        }

        _logger.LogInformation("Profile update attempt by {Username} at {Time}", username, DateTime.UtcNow);

        try
        {
            await _userService.UpdateUserByUsernameAsync(username, request);
            _logger.LogInformation("Profile updated successfully for {Username} at {Time}", username, DateTime.UtcNow);
            return Ok(new { message = "Profile updated successfully" });
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning("Profile update failed for {Username}: {Message}", username, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for {Username}", username);
            return StatusCode(500, new { message = "An error occurred while updating the profile" });
        }
    }
    
    // PUT: api/user/{id} working
    // Access: Admin
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        _logger.LogInformation("=== Admin UpdateUser Start ===");
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUsername = User.Identity?.Name;
        
        _logger.LogInformation("Admin user {AdminUsername} (ID: {AdminId}) updating user ID {TargetId} at {Time}", 
            currentUsername, currentUserId, id, DateTime.UtcNow);
        _logger.LogDebug("Update request data: {@RequestData}", request);
        _logger.LogDebug("Current user roles: {Roles}", 
            string.Join(", ", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));

        try
        {
            await _userService.UpdateUserAsync(id, request);
            _logger.LogInformation("User ID {Id} updated successfully by admin {AdminId} at {Time}", 
                id, currentUserId, DateTime.UtcNow);
            return Ok(new { message = "User updated successfully" });
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning("Failed to update user ID {Id}: {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the user" });
        }
    }
    
    private async Task<(string Username, string UserId, IList<string> Roles)> GetCurrentUserInfo()
    {
        var username = User.Identity?.Name;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
            
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Missing required claims in token");
            throw new UnauthorizedAccessException("Invalid user identity: Missing required claims");
        }
        
        return (username, userId, roles);
    }

    public class UpdateUserRequest
    {
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Biography { get; set; }
        public string? Nif { get; set; }
        public string? UserName { get; set; }
    }
}
