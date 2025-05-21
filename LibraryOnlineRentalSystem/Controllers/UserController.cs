using Microsoft.AspNetCore.Authorization;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Repository.RoleRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryOnlineRentalSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IRoleRepository _roleRepository;

    public UserController(UserService userService, IRoleRepository roleRepository)
    {
        _userService = userService;
        _roleRepository = roleRepository;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] NewUserDTO request)
    {
        try
        {
            await _userService.CreateUserAsync(request);
            return Ok(new { message = "User registered successfully" });
        }
        catch (BusinessRulesException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] NewUserDTO request)
    {
        try
        {
            await _userService.CreateUserAsync(request);
            return Ok("User created successfully.");
        }
        catch (BusinessRulesException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
   
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (User.IsInRole("Admin") || currentUserId == id.ToString())
        {
            await _userService.UpdateUserAsync(id, request);
            return Ok("User updated.");
        }

        return Forbid();
    }

    //[HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]
    //public async Task<IActionResult> DeleteUser(Guid id)
    //{
      //  await _userService.DeleteUserAsync(id);
        //return Ok("User deleted.");
    //}

    public class UpdateUserRequest
    {
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Biography { get; set; }
        public string? RoleId { get; set; } // Only Admin should use this
    }
}
