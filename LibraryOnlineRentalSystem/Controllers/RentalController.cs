using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Rentals;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LibraryOnlineRentalSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalController : ControllerBase
{
    private readonly RentalService _rentalService;
    private readonly UserService _userService;
    private readonly BookService _bookService;
    private readonly ILogger<RentalController> _logger;

    public RentalController(
        RentalService rentalService, 
        UserService userService, 
        BookService bookService, 
        ILogger<RentalController> logger)
    {
        _rentalService = rentalService;
        _userService = userService;
        _bookService = bookService;
        _logger = logger;
    }
    
    [HttpPost]
    //[Authorize(Roles = "LibraryManager")]
    public async Task<IActionResult> CreateRental(CreatedRentalDTO dto)
    {
        if (dto == null)
        {
            _logger.LogWarning("Create rental failed: Request body is null");
            return BadRequest("Rental data is required");
        }

        try
        {
            _logger.LogInformation("Rental creation requested by {UserEmail} for book {BookId}", 
                dto.UserEmail, dto.ReservedBookId);

            // Parse dates
            if (!DateTime.TryParse(dto.StartDate, out var startDate) || 
                !DateTime.TryParse(dto.EndDate, out var endDate))
            {
                _logger.LogWarning("Invalid date format in rental request");
                return BadRequest("Invalid date format. Please use ISO 8601 format (e.g., 2023-01-01T00:00:00Z)");
            }

            // Basic date validation
            if (endDate < startDate)
            {
                _logger.LogWarning("End date {EndDate} is before start date {StartDate}", endDate, startDate);
                return BadRequest("End date cannot be before start date");
            }

            try
            {
                // Create the rental (all validations are handled in the service layer)
                var rental = await _rentalService.CreateARentalAsync(dto);

                // Log the successful rental creation
                _logger.LogInformation("Rental created with ID {RentalId} for user {UserEmail}", 
                    rental.IdRentalValue, dto.UserEmail);

                // Log to file with formatted rental details
                var rentalDetails = $"Rental Created\n" +
                    $"--------INVOICE--------\n" +
                    $"Rental ID: {rental.IdRentalValue}\n" +
                    $"User Email: {rental.UserEmail}\n" +
                    $"Book ID: {rental.ReservedBookId}\n" +
                    $"Start Date: {rental.StartDate}\n" +
                    $"End Date: {rental.EndDate}\n" +
                    $"Status: {rental.RentalStatus}\n" +
                    $"Created At: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"----------------";
                FilePrint.RentalFilePrint(rentalDetails);

                return Ok(new { Message = "Rental created successfully" });
            }
            catch (BusinessRulesException ex) when (ex.Message.Contains("No available copies", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Failed to create rental - {Message}", ex.Message);
                return Conflict(ex.Message);
            }
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning(ex, "Business rule violation: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating rental");
            return StatusCode(500, "An unexpected error occurred while processing your request");
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<RentalDTO>> GetRentalById(string id)
    {
        _logger.LogInformation("Fetching rental with ID {Id} at {Time}", id, DateTime.UtcNow);
        
        try
        {
            _logger.LogDebug("Calling _rentalService.GetRentalByIdAsync with ID: {Id}", id);
            var rental = await _rentalService.GetRentalByIdAsync(id);
            
            if (rental == null)
            {
                _logger.LogWarning("Rental with ID {Id} not found at {Time}", id, DateTime.UtcNow);
                return NotFound($"Rental with ID {id} not found");
            }
            
            _logger.LogInformation("Successfully retrieved rental with ID {Id} at {Time}", id, DateTime.UtcNow);
            return rental;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching rental with ID {Id} at {Time}. Error: {ErrorMessage}", 
                id, DateTime.UtcNow, ex.Message);
            _logger.LogDebug(ex, "Full exception details for rental ID {Id}", id);
            return BadRequest($"An error occurred while fetching rental: {ex.Message}");
        }
    }

    // PUT: api/rental/{id}/status
    [HttpPut("{id}/status")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<IActionResult> UpdateRentalStatus(string id, [FromBody] UpdateRentalStatusDTO statusDto)
    {
        _logger.LogInformation("Updating status for rental ID {Id} at {Time}", id, DateTime.UtcNow);
        
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Attempted to update status with null or empty rental ID");
            return BadRequest(new { message = "Rental ID is required" });
        }

        if (statusDto == null || string.IsNullOrWhiteSpace(statusDto.Status))
        {
            _logger.LogWarning("Attempted to update rental {RentalId} with empty status", id);
            return BadRequest(new { message = "Status is required" });
        }
        
        // Validate status value
        var validStatuses = new[] { "active", "pending", "completed", "cancelled" };
        if (!validStatuses.Contains(statusDto.Status.ToLower()))
        {
            _logger.LogWarning("Attempted to update rental {RentalId} with invalid status: {Status}", id, statusDto.Status);
            return BadRequest(new { message = $"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}" });
        }
        
        try
        {
            await _rentalService.UpdateRentalStatusAsync(id, statusDto.Status);
            _logger.LogInformation("Successfully updated status for rental ID {Id} to {Status}", id, statusDto.Status);
            return Ok(new { message = $"Status of rental with id {id} changed to {statusDto.Status.ToLower()}" });
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning("Status update failed for rental {RentalId}: {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for rental ID {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the rental status" });
        }
    }

    // GET: api/rental/user/{email} 
    [HttpGet("user/{email}")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllRentalsOfUser(string email)
    {
        _logger.LogInformation("Fetching rentals for user {Email} at {Time}", email, DateTime.UtcNow);
        
        try
        {
            var rentals = await _rentalService.GetAllRentalsOfUserAsync(email);
            
            if (rentals == null || !rentals.Any())
            {
                _logger.LogInformation("No rentals found for user {Email}", email);
                return Ok(new { message = $"No rentals found for user {email}" });
            }
            
            return rentals;
        }
        catch (BusinessRulesException ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("User not found: {Email}", email);
            return NotFound(new { message = $"User with email {email} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid email format: {Email}", email);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching rentals for user {Email}", email);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while fetching rentals" });
        }
    }

    // GET: api/rental/user/{email}/active
    [HttpGet("user/{email}/active")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllActiveRentalsOfUser(string email)
    {
        return await HandleUserRentalsRequest(email, "active", 
            () => _rentalService.GetAllActiveRentalsOfUserAsync(email));
    }

    // GET: api/rental/user/{email}/cancelled
    [HttpGet("user/{email}/cancelled")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCancelledRentalsOfUser(string email)
    {
        return await HandleUserRentalsRequest(email, "cancelled", 
            () => _rentalService.GetAllCancelledRentalsOfUserAsync(email));
    }

    // GET: api/rental/user/{email}/pending
    [HttpGet("user/{email}/pending")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllPendingRentalsOfUser(string email)
    {
        return await HandleUserRentalsRequest(email, "pending", 
            () => _rentalService.GetAllPendingRentalsOfUserAsync(email));
    }

    // GET: api/rental/user/{email}/completed

    [HttpGet("user/{email}/completed")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCompletedRentalsOfUser(string email)
    {
        return await HandleUserRentalsRequest(email, "completed", 
            () => _rentalService.GetAllCompletedRentalsOfUserAsync(email));
    }
    
    private async Task<ActionResult<List<RentalDTO>>> HandleUserRentalsRequest(string email, string rentalType, 
        Func<Task<List<RentalDTO>>> getRentalsFunc)
    {
        _logger.LogInformation("Fetching {RentalType} rentals for user {Email} at {Time}", 
            rentalType, email, DateTime.UtcNow);
            
        try
        {
            var rentals = await getRentalsFunc();
            
            if (rentals == null || !rentals.Any())
            {
                _logger.LogInformation("No {RentalType} rentals found for user {Email}", rentalType, email);
                return Ok(new { message = $"No {rentalType} rentals found for user {email}" });
            }
            
            return rentals;
        }
        catch (BusinessRulesException ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("User not found: {Email}", email);
            return NotFound(new { message = $"User with email {email} not found" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid email format: {Email}", email);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching {RentalType} rentals for user {Email}", rentalType, email);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = $"An error occurred while fetching {rentalType} rentals" });
        }
    }

    // GET: api/rental
    [HttpGet]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllRentals()
    {
        _logger.LogInformation("Fetching all rentals stored at {Time}", DateTime.UtcNow);

        var rentals = await _rentalService.GetAllRentalsAsync();
        return rentals;
    }

    // GET: api/rental/active
    [HttpGet("active")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllActiveRentals()
    {
        _logger.LogInformation("Fetching all active rentals at {Time}", DateTime.UtcNow);

        var rentals = await _rentalService.GetAllActiveRentalsAsync();
        return rentals;
    }

    // GET: api/rental/cancelled
    [HttpGet("cancelled")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCancelledRentals()
    {
        _logger.LogInformation("Fetching all cancelled rentals at {Time}", DateTime.UtcNow);

        var rentals = await _rentalService.GetAllCancelledRentalsAsync();
        return rentals;
    }

    // GET: api/rental/pending
    [HttpGet("pending")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllPendingRentals()
    {
        _logger.LogInformation("Fetching all pending rentals at {Time}", DateTime.UtcNow);

        var rentals = await _rentalService.GetAllPendingRentalsAsync();
        return rentals;
    }

    // GET: api/rental/completed
    [HttpGet("completed")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCompletedRentals()
    {
        _logger.LogInformation("Fetching all completed rentals at {Time}", DateTime.UtcNow);

        var rentals = await _rentalService.GetAllCompletedRentalsAsync();
        return rentals;
    }
}