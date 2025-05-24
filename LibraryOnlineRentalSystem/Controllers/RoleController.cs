using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryOnlineRentalSystem.Repository.Common;

namespace LibraryOnlineRentalSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public RoleController(LibraryDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllRoles()
    {
        var roles = _context.Roles
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Description
            })
            .ToList();

        return Ok(roles);
    }
}