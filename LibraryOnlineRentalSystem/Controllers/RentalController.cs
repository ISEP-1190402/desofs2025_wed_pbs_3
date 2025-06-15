using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Rentals;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Repository.RentalRepository;
using LibraryOnlineRentalSystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;

namespace LibraryOnlineRentalSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalController : ControllerBase
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IWorkUnity _workUnit;
    private readonly ILogger<RentalController> _logger;
    private readonly RentalService _rentalService;

    public RentalController(
        IRentalRepository rentalRepository,
        IUserRepository userRepository,
        IBookRepository bookRepository,
        IWorkUnity workUnit,
        ILogger<RentalController> logger,
        RentalService rentalService)
    {
        _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        _workUnit = workUnit ?? throw new ArgumentNullException(nameof(workUnit));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _rentalService = rentalService ?? throw new ArgumentNullException(nameof(rentalService));
    }
    
    private async Task<(string Username, string UserId, IList<string> Roles, string Email)> GetCurrentUserInfo()
    {
        var username = User.Identity?.Name;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
            
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Missing required claims in token");
            throw new UnauthorizedAccessException("Invalid user identity: Missing required claims");
        }
        
        return (username, userId, roles, email);
    }

    public class CreateRentalRequestDto
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Isbn { get; set; }
        public string UserEmail { get; set; }
    }

    // POST: api/rental working
    // Access: User, LibraryManager
    [HttpPost]
    [Authorize(Roles = "User,LibraryManager")]
    public async Task<IActionResult> CreateRental([FromBody] CreateRentalRequestDto dto)
    {
        _logger.LogInformation("=== CreateRental Start ===");
        
        if (User.IsInRole("Admin"))
        {
            _logger.LogWarning("Admin user attempted to create rental at {Time}", DateTime.UtcNow);
            return Forbid("Admins are not allowed to rent books.");
        }

        if (dto == null)
        {
            _logger.LogWarning("Create rental failed: Request body is null");
            return BadRequest(new { message = "Rental data is required" });
        }

        _logger.LogInformation("Request data: {Data}", JsonSerializer.Serialize(dto));

        try
        {
        
            var currentUser = await GetCurrentUserInfo();
            _logger.LogInformation("Current user: {Email}, Roles: {Roles}", 
                currentUser.Email, string.Join(", ", currentUser.Roles));

            // Validate email
            if (string.IsNullOrEmpty(dto.UserEmail))
            {
                _logger.LogWarning("User email is required in the request body");
                return BadRequest(new { message = "User email is required" });
            }

            // Check if user has permission to create rental for the specified email
            if (!User.IsInRole("LibraryManager") && 
                !dto.UserEmail.Equals(currentUser.Email, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("User {UserEmail} is not authorized to create rental for {RequestedEmail}", 
                    currentUser.Email, dto.UserEmail);
                return Forbid("You can only create rentals for your own account");
            }

            // Validate dates
            if (!DateTime.TryParse(dto.StartDate, out var startDate) || 
                !DateTime.TryParse(dto.EndDate, out var endDate))
            {
                _logger.LogWarning("Invalid date format. Start: {StartDate}, End: {EndDate}", 
                    dto.StartDate, dto.EndDate);
                return BadRequest(new { message = "Invalid date format. Use ISO 8601 format (e.g., 2023-01-01T00:00:00)" });
            }

            if (endDate <= startDate)
            {
                _logger.LogWarning("End date {EndDate} is before or equal to start date {StartDate}", 
                    endDate, startDate);
                return BadRequest(new { message = "End date cannot be before or equal to start date" });
            }

            var book = await _bookRepository.GetBookByIsbnAsync(dto.Isbn);
            if (book == null)
            {
                _logger.LogWarning("Book with ISBN {Isbn} not found", dto.Isbn);
                return NotFound(new { message = $"Book with ISBN {dto.Isbn} not found" });
            }

            var user = await _userRepository.GetByEmailAsync(dto.UserEmail);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", dto.UserEmail);
                return NotFound(new { message = $"User with email {dto.UserEmail} not found" });
            }

            // Check if there are available copies
            var availableCopies = book.AmountOfCopies.GetBookAmountOfCopies();
            if (availableCopies <= 0)
            {
                _logger.LogWarning("No available copies of book with ISBN {Isbn}", dto.Isbn);
                return BadRequest(new { message = "No available copies of the book" });
            }

            var rentalDto = new CreatedRentalDTO(
                dto.StartDate,
                dto.EndDate,
                book.Id.Value, 
                dto.UserEmail
            );
            var updatedBook = _bookRepository.UpdateBookStock(book.Id.Value, availableCopies - 1);
            if (updatedBook == null)
            {
                _logger.LogError("Failed to update book stock for book with ISBN {Isbn}", dto.Isbn);
                return StatusCode(500, new { message = "Failed to update book stock" });
            }
            
            var rental = await _rentalService.CreateARentalAsync(rentalDto);

            // Log the successful rental creation
            _logger.LogInformation("Rental created with ID {RentalId} for user {UserEmail}", 
                rental.IdRentalValue, currentUser.Email);

            // Log to file with formatted rental details
            var rentalDetails = $"Rental Created\n" +
                $"--------INVOICE--------\n" +
                $"Rental ID: {rental.IdRentalValue}\n" +
                $"User Email: {currentUser.Email}\n" +
                $"Book ID: {dto.Isbn}\n" +
                $"Start Date: {dto.StartDate}\n" +
                $"End Date: {dto.EndDate}\n" +
                $"Status: {rental.RentalStatus}\n" +
                $"Created At: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                $"----------------";
            FilePrint.RentalFilePrint(rentalDetails);

            return Ok(rental);
        }
        catch (Exception ex) when (ex is JsonException || ex is ArgumentException)
        {
            _logger.LogWarning(ex, "Invalid request data");
            return BadRequest(new { message = "Invalid request data" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating rental");
            return StatusCode(500, new { message = "An unexpected error occurred while processing your request" });
        }
        finally
        {
            _logger.LogInformation("=== CreateRental End ===");
        }
    }
    
    
    // GET: api/rental/user/{username}/rentals working get all rentals of user
    // Access: Any authenticated user (own rentals), Admin/LibraryManager (any rentals)
    [HttpGet("user/{username}/rentals")]
    [Authorize]
    public async Task<ActionResult<List<RentalDTO>>> GetRentalsByUsername(string username)
    {
        _logger.LogInformation("=== GetRentalsByUsername Start ===");
        _logger.LogInformation("Fetching rentals for username: {Username}", username);
        
        if (string.IsNullOrWhiteSpace(username))
        {
            _logger.LogWarning("Username cannot be empty");
            return BadRequest(new { message = "Username is required" });
        }
        
        try
        {
            var currentUser = await GetCurrentUserInfo();
            _logger.LogDebug("Current user: {CurrentUser}, Requested username: {RequestedUser}", 
                currentUser.Username, username);
            
            // Users can only view their own rentals unless they are admins or library managers
            if (!User.IsInRole("Admin") && !User.IsInRole("LibraryManager") && 
                !string.Equals(currentUser.Username, username, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("User {CurrentUser} is not authorized to view rentals for {RequestedUser}", 
                    currentUser.Username, username);
                return Forbid("You can only view your own rentals");
            }
            
            // Get user by username to verify they exist and get their email
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("User with username {Username} not found", username);
                return NotFound(new { message = $"User with username {username} not found" });
            }
            
            // Get rentals for the user by email
            var rentals = await _rentalRepository.GetAllAsyncOfUser(user.Email.EmailAddress);
            _logger.LogInformation("Found {Count} rentals for user {Username}", 
                rentals.Count, username);
                
            // Convert to DTOs
            var rentalDtos = rentals.Select(r => new RentalDTO(
                r.Id.Value,
                r.StartDate.StartDateTime.ToString("o"),
                r.EndDate.EndDateTime.ToString("o"),
                r.RentedBookIdentifier.BookId,
                r.StatusOfRental.RentStatus.ToString(),
                r.EmailUser.EmailAddress
            )).ToList();
                
            return Ok(rentalDtos);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while fetching rentals for {Username}", username);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching rentals for user {Username}", username);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while fetching the rentals" });
        }
    }
    
    
    // GET: api/rental/user/{username}/rentals/{id} working
    // Access: Regular users (own rental only) get specific rental of user
    [HttpGet("user/{username}/rentals/{id}")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<RentalDTO>> GetUserRental(string username, string id)
    {
        _logger.LogInformation("=== GetUserRental Start ===");
        _logger.LogInformation("Fetching rental with ID {RentalId} for user: {Username}", id, username);
            
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Username and rental ID are required");
            return BadRequest(new { message = "Username and rental ID are required" });
        }
        
        try
        {
            // Immediately block Admin/LibraryManager access
            if (User.IsInRole("Admin") || User.IsInRole("LibraryManager"))
            {
                _logger.LogWarning("Admin/LibraryManager access denied to GetUserRental");
                return BadRequest(new { message = "Not allowed" });
            }
            
            var currentUser = await GetCurrentUserInfo();
            
            // Verify the requesting user is accessing their own rentals
            if (!string.Equals(currentUser.Username, username, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("User {CurrentUser} is not authorized to access rentals for {RequestedUser}", 
                    currentUser.Username, username);
                return NotFound(new { message = "Cannot find" });
            }
            
            // Get the rental by ID
            var rental = await _rentalRepository.GetByIdAsync(new RentalID(id));
            if (rental == null)
            {
                _logger.LogWarning("Rental with ID {RentalId} not found", id);
                return NotFound(new { message = $"Rental with ID {id} not found" });
            }
            
            // Verify the rental belongs to the requesting user
            if (!string.Equals(rental.EmailUser.EmailAddress, currentUser.Email, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("User {CurrentUser} attempted to access rental {RentalId} belonging to {RentalUser}",
                    currentUser.Username, id, rental.EmailUser.EmailAddress);
                return NotFound(new { message = "Cannot find" });
            }
            
            _logger.LogInformation("Successfully retrieved rental with ID: {RentalId} for user: {Username}", 
                id, username);
                
            var rentalDto = new RentalDTO(
                rental.Id.Value,
                rental.StartDate.StartDateTime.ToString("o"),
                rental.EndDate.EndDateTime.ToString("o"),
                rental.RentedBookIdentifier.BookId,
                rental.StatusOfRental.RentStatus.ToString(),
                rental.EmailUser.EmailAddress
            );
            
            return Ok(rentalDto);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is FormatException)
        {
            _logger.LogWarning(ex, "Invalid rental ID format: {RentalId}", id);
            return BadRequest(new { message = "Invalid rental ID format" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to rental with ID: {RentalId}", id);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching rental with ID: {RentalId} for user: {Username}", 
                id, username);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while fetching the rental" });
        }
        finally
        {
            _logger.LogInformation("=== GetUserRental End ===");
        }
    }

    // GET: api/rental/{id} working
    // Access: Admin, LibraryManager
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<ActionResult<RentalDTO>> GetRentalById(string id)
    {
        _logger.LogInformation("=== GetRentalById Start ===");
        _logger.LogInformation("Fetching rental with ID: {RentalId}", id);
        
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Rental ID cannot be empty");
            return BadRequest(new { message = "Rental ID is required" });
        }

        try
        {
            var currentUser = await GetCurrentUserInfo();
            _logger.LogDebug("User {Username} is fetching rental with ID: {RentalId}", 
                currentUser.Username, id);
            
            var rental = await _rentalRepository.GetByIdAsync(new RentalID(id));
            
            if (rental == null)
            {
                _logger.LogWarning("Rental with ID {RentalId} not found", id);
                return NotFound(new { message = $"Rental with ID {id} not found" });
            }
            
            _logger.LogInformation("Successfully retrieved rental with ID: {RentalId}", id);
            
            var rentalDto = new RentalDTO(
                rental.Id.Value,
                rental.StartDate.StartDateTime.ToString("o"),
                rental.EndDate.EndDateTime.ToString("o"),
                rental.RentedBookIdentifier.BookId,
                rental.StatusOfRental.RentStatus.ToString(),
                rental.EmailUser.EmailAddress
            );
            
            return Ok(rentalDto);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is FormatException)
        {
            _logger.LogWarning(ex, "Invalid rental ID format: {RentalId}", id);
            return BadRequest(new { message = "Invalid rental ID format" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to rental with ID: {RentalId}", id);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching rental with ID: {RentalId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while fetching the rental" });
        }
        finally
        {
            _logger.LogInformation("=== GetRentalById End ===");
        }
    }
    
    // PUT: api/rental/{id}/status working
    // Access: LibraryManager
    [HttpPut("{id}/status")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<IActionResult> UpdateRentalStatus(string id, [FromBody] UpdateRentalStatusDTO statusDto)
    {
        _logger.LogInformation("=== UpdateRentalStatus Start ===");
        _logger.LogInformation("Updating status for rental ID: {RentalId}", id);
        
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Rental ID cannot be empty");
            return BadRequest(new { message = "Rental ID is required" });
        }

        if (statusDto == null || string.IsNullOrWhiteSpace(statusDto.Status))
        {
            _logger.LogWarning("Status is required");
            return BadRequest(new { message = "Status is required" });
        }
        
        // Validate status value
        var validStatuses = new[] { "active", "pending", "completed", "cancelled" };
        var status = statusDto.Status.ToLower();
        if (!validStatuses.Contains(status))
        {
            _logger.LogWarning("Invalid status value: {Status}", status);
            return BadRequest(new { 
                message = $"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}" 
            });
        }
        
        try
        {
            var currentUser = await GetCurrentUserInfo();
            _logger.LogDebug("User {Username} is updating status of rental {RentalId} to {Status}", 
                currentUser.Username, id, status);
            
            var rental = await _rentalRepository.GetByIdAsync(new RentalID(id));
            if (rental == null)
            {
                _logger.LogWarning("Rental with ID {RentalId} not found", id);
                return NotFound(new { message = $"Rental with ID {id} not found" });
            }
            
            // Update status based on the provided value
            switch (status)
            {
                case "active":
                    rental.MarkAsActive();
                    break;
                case "pending":
                    rental.MarkAsPending();
                    break;
                case "completed":
                    rental.MarkAsCompleted();
                    break;
                case "cancelled":
                    rental.CancelBooking();
                    break;
            }
            
            _rentalRepository.Update(rental);
            await _workUnit.CommitAsync();
            
            _logger.LogInformation("Successfully updated status of rental {RentalId} to {Status}", id, status);
            
            var rentalDto = new RentalDTO(
                rental.Id.Value,
                rental.StartDate.StartDateTime.ToString("o"),
                rental.EndDate.EndDateTime.ToString("o"),
                rental.RentedBookIdentifier.BookId,
                rental.StatusOfRental.RentStatus.ToString(),
                rental.EmailUser.EmailAddress
            );
            
            return Ok(new { 
                message = $"Status of rental with ID {id} updated to {status}",
                rental = rentalDto
            });
        }
        catch (Exception ex) when (ex is ArgumentException || ex is FormatException)
        {
            _logger.LogWarning(ex, "Invalid rental ID format: {RentalId}", id);
            return BadRequest(new { message = "Invalid rental ID format" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to update rental with ID: {RentalId}", id);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for rental ID: {RentalId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while updating the rental status" });
        }
        finally
        {
            _logger.LogInformation("=== UpdateRentalStatus End ===");
        }
    }
    
    // GET: api/rental/user/{email} working
    // Access: Admin, LibraryManager
    [HttpGet("user/{email}")]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetRentalsByUserEmail(string email)
    {
        return await HandleUserRentalsRequest(email, "all", 
            rentals => rentals.ToList());
    }
    
    // GET: api/rental/user/{email}/active working
    // Access: Admin, LibraryManager
    [HttpGet("user/{email}/active")] 
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetActiveRentalsByUserEmail(string email)
    {
        return await HandleUserRentalsRequest(email, "active", 
            rentals => rentals.Where(r => r.GetCurrentStatus().RentStatus == Status.Active).ToList());
    }

    // GET: api/rental/user/{email}/cancelled working
    // Access: Admin, LibraryManager
    [HttpGet("user/{email}/cancelled")]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetCancelledRentalsByUserEmail(string email)
    {
        return await HandleUserRentalsRequest(email, "cancelled", 
            rentals => rentals.Where(r => r.GetCurrentStatus().RentStatus == Status.Cancelled).ToList());
    }

    // GET: api/rental/user/{email}/pending working
    // Access: Admin, LibraryManager
    [HttpGet("user/{email}/pending")]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetPendingRentalsByUserEmail(string email)
    {
        return await HandleUserRentalsRequest(email, "pending", 
            rentals => rentals.Where(r => r.GetCurrentStatus().RentStatus == Status.Pending).ToList());
    }
    
    // GET: api/rental/user/{email}/completed working
    // Access: Admin, LibraryManager
    [HttpGet("user/{email}/completed")]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetCompletedRentalsByUserEmail(string email)
    {
        return await HandleUserRentalsRequest(email, "completed", 
            rentals => rentals.Where(r => r.GetCurrentStatus().RentStatus == Status.Completed).ToList());
    }
    
    private async Task<ActionResult<List<RentalDTO>>> HandleUserRentalsRequest(string email, string rentalType, 
        Func<List<Domain.Rentals.Rental>, List<Domain.Rentals.Rental>> filterRentals)
    {
        _logger.LogInformation("=== HandleUserRentalsRequest Start ===");
        _logger.LogInformation("Fetching {RentalType} rentals for user: {Email}", rentalType, email);
            
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("Email is required");
            return BadRequest(new { message = "Email is required" });
        }
        
        try
        {
            var currentUser = await GetCurrentUserInfo();
            
            // Only allow admins/library managers to view other users' rentals
            if (!User.IsInRole("Admin") && !User.IsInRole("LibraryManager") && 
                !string.Equals(currentUser.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("User {CurrentUser} is not authorized to access {RentalType} rentals for {Email}", 
                    currentUser.Email, rentalType, email);
                return Forbid("You can only view your own rentals");
            }
            
            // Get user by email to verify they exist
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return NotFound(new { message = $"User with email {email} not found" });
            }
            
            // Get all rentals for the user and apply the filter
            var allRentals = await _rentalRepository.GetAllAsyncOfUser(email);
            var filteredRentals = filterRentals(allRentals);
            
            _logger.LogInformation("Found {Count} {RentalType} rentals for user {Email}", 
                filteredRentals.Count, rentalType, email);
                
            var rentalDtos = filteredRentals.Select(r => new RentalDTO(
                r.Id.Value,
                r.StartDate.StartDateTime.ToString("o"),
                r.EndDate.EndDateTime.ToString("o"),
                r.RentedBookIdentifier.BookId,
                r.StatusOfRental.RentStatus.ToString(),
                r.EmailUser.EmailAddress
            )).ToList();
            
            return Ok(rentalDtos);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to fetch {RentalType} rentals for user: {Email}", 
                rentalType, email);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching {RentalType} rentals for user: {Email}", 
                rentalType, email);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = $"An error occurred while fetching {rentalType} rentals" });
        }
        finally
        {
            _logger.LogInformation("=== HandleUserRentalsRequest End ===");
        }
    }

    // GET: api/rental working
    // Access: admin, LibraryManager
    [HttpGet]
    [Authorize(Roles = "Admin, LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllRentals()
    {
        _logger.LogInformation("=== GetAllRentals Start ===");
        _logger.LogInformation("Fetching all rentals");

        try
        {
            var currentUser = await GetCurrentUserInfo();
            _logger.LogDebug("User {Username} is fetching all rentals", currentUser.Username);
            
            var rentals = await _rentalRepository.GetAllAsync();
            _logger.LogInformation("Successfully retrieved {Count} rentals", rentals.Count);
            
            return Ok(rentals.Select(r => r.toDTO()).ToList());
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to fetch all rentals");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all rentals");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while fetching all rentals" });
        }
        finally
        {
            _logger.LogInformation("=== GetAllRentals End ===");
        }
    }

    // GET: api/rental/active working
    // Access: LibraryManager
    [HttpGet("active")] 
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllActiveRentals()
    {
        _logger.LogInformation("=== GetAllActiveRentals Start ===");
        _logger.LogInformation("Fetching all active rentals");

        try
        {
            var currentUser = await GetCurrentUserInfo();
            _logger.LogDebug("User {Username} is fetching all active rentals", currentUser.Username);
            
            // Get all rentals and filter by status
            var allRentals = await _rentalRepository.GetAllActiveRentalsAssync();
            
            _logger.LogInformation("Successfully retrieved {Count} active rentals", allRentals.Count);
            
            // Convert to DTOs
            var rentalDtos = allRentals.Select(r => new RentalDTO(
                r.Id.Value,
                r.StartDate.StartDateTime.ToString("o"),
                r.EndDate.EndDateTime.ToString("o"),
                r.RentedBookIdentifier.BookId,
                r.StatusOfRental.RentStatus.ToString(),
                r.EmailUser.EmailAddress
            )).ToList();
            
            return Ok(rentalDtos);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to fetch active rentals");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching active rentals");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while fetching active rentals" });
        }
        finally
        {
            _logger.LogInformation("=== GetAllActiveRentals End ===");
        }
    }
    
    // GET: api/rental/cancelled working
    // Access: LibraryManager
    [HttpGet("cancelled")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCancelledRentals()
    {
        _logger.LogInformation("=== GetAllCancelledRentals Start ===");
        _logger.LogInformation("Fetching all cancelled rentals");

        try
        {
            var currentUser = await GetCurrentUserInfo();
            _logger.LogDebug("User {Username} is fetching all cancelled rentals", currentUser.Username);
            
            var rentals = await _rentalRepository.GetAllCancelledRentalsAssync();
            var rentalDtos = rentals.Select(r => new RentalDTO(
                r.Id.Value,
                r.StartDate.StartDateTime.ToString("o"),
                r.EndDate.EndDateTime.ToString("o"),
                r.RentedBookIdentifier.BookId,
                r.StatusOfRental.RentStatus.ToString(),
                r.EmailUser.EmailAddress
            )).ToList();
            
            _logger.LogInformation("Successfully retrieved {Count} cancelled rentals", rentalDtos.Count);
            return Ok(rentalDtos);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to fetch cancelled rentals");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching cancelled rentals");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while fetching cancelled rentals" });
        }
        finally
        {
            _logger.LogInformation("=== GetAllCancelledRentals End ===");
        }
    }
    
    // GET: api/rental/pending working
    // Access: LibraryManager
    [HttpGet("pending")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllPendingRentals()
    {
        _logger.LogInformation("=== GetAllPendingRentals Start ===");
        _logger.LogInformation("Fetching all pending rentals");

        try
        {
            var currentUser = await GetCurrentUserInfo();
            _logger.LogDebug("User {Username} is fetching all pending rentals", currentUser.Username);
            
            var rentals = await _rentalRepository.GetAllPendingRentalsAssync();
            var rentalDtos = rentals.Select(r => new RentalDTO(
                r.Id.Value,
                r.StartDate.StartDateTime.ToString("o"),
                r.EndDate.EndDateTime.ToString("o"),
                r.RentedBookIdentifier.BookId,
                r.StatusOfRental.RentStatus.ToString(),
                r.EmailUser.EmailAddress
            )).ToList();
            return Ok(rentalDtos);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to fetch pending rentals");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pending rentals");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while fetching pending rentals" });
        }
        finally
        {
            _logger.LogInformation("=== GetAllPendingRentals End ===");
        }
    }
    
    // GET: api/rental/completed working
    // Access: Admin, LibraryManager
    [HttpGet("completed")]
    [Authorize(Roles = "Admin,LibraryManager")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCompletedRentals()
    {
        _logger.LogInformation("=== GetAllCompletedRentals Start ===");
        _logger.LogInformation("Fetching all completed rentals");

        try
        {
            var currentUser = await GetCurrentUserInfo();
            _logger.LogDebug("User {Username} is fetching all completed rentals", currentUser.Username);
            
            var rentals = await _rentalRepository.GetAllCompletedRentalsAssync();
            var rentalDtos = rentals.Select(r => new RentalDTO(
                r.Id.Value,
                r.StartDate.StartDateTime.ToString("o"),
                r.EndDate.EndDateTime.ToString("o"),
                r.RentedBookIdentifier.BookId,
                r.StatusOfRental.RentStatus.ToString(),
                r.EmailUser.EmailAddress
            )).ToList();
            
            _logger.LogInformation("Successfully retrieved {Count} completed rentals", rentalDtos.Count);
            return Ok(rentalDtos);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to fetch completed rentals");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching completed rentals");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while fetching completed rentals" });
        }
        finally
        {
            _logger.LogInformation("=== GetAllCompletedRentals End ===");
        }
    }
}