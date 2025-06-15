using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Rentals;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LibraryOnlineRentalSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalController : ControllerBase
{
    private readonly RentalService _rentalService;
    private readonly UserService _userService;
    private readonly BookService _bookService;

    private readonly ILogger<RentalController> _logger;

    public RentalController(RentalService rentalService, UserService userService, BookService bookService, ILogger<RentalController> logger)
    {
        _rentalService = rentalService;
        _userService = userService;
        _bookService = bookService;
        _logger = logger;
    }

    // POST: api/rental
    [HttpPost]
    public async Task<ActionResult<RentalDTO>> CreateRental(CreatedRentalDTO dto)
    {
        try
        {
            _logger.LogInformation("Rental creation requested at {Time}", DateTime.UtcNow);

            var startDate = DateTime.Parse(dto.StartDate);
            var endDate = DateTime.Parse(dto.EndDate);

            if (endDate < startDate)
            {
                // Retorna BadRequest com uma mensagem explicativa
                return BadRequest("End date cannot be before start date.");
            }
        }
        catch (Exception e)
        {
            // Retorna BadRequest se houver erro ao converter datas
            return BadRequest("Invalid date format.");
        }


        /*var ammountOfBooks = _bookService.GetAmmountOfBooks(new BookID(dto.ReservedBookId));
        var availableAmmountOfBooks = _rentalService.GetBusyAmmountOfBooks(
                new RentedBookID(dto.ReservedBookId),
                new RentalStartDate(DateTime.Parse(dto.StartDate)),
                new RentalEndDate(DateTime.Parse(dto.EndDate)));

        if (ammountOfBooks - availableAmmountOfBooks == 0)
        {
            return Forbid("We don't have enough books available.");
        }*/


        var rental = await _rentalService.CreateARentalAsync(dto);

        FilePrint.RentalFilePrint(dto.ToString());
        Console.WriteLine(rental.ToString());

        _logger.LogInformation("Rental created with ID {Id} at {Time}", rental.IdRentalValue, DateTime.UtcNow);
        return rental;
    }

    // PUT: api/rental/cancel/{id}
    [HttpPut("cancel/{id}")]
    public async Task<ActionResult<RentalDTO>> CancelRental(string id)
    {
        _logger.LogInformation("Rental cancel requested for ID {Id} at {Time}", id, DateTime.UtcNow);
        var rental = await _rentalService.CancelARental(id);
        if (rental == null)
        {
            _logger.LogWarning("Rental cancel failed - ID {Id} not found at {Time}", id, DateTime.UtcNow);
            return NotFound("Rental not found.");
        }
        _logger.LogInformation("Rental cancelled for ID {Id} at {Time}", id, DateTime.UtcNow);
        return rental;
    }

    // GET: api/rental/user/{email} 
    [HttpGet("user/{email}")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllRentalsOfUser(string email)
    {
        _logger.LogInformation("Fetching rentals for user {Email} at {Time}", email, DateTime.UtcNow);
        var rentals = await _rentalService.GetAllRentalsOfUserAsync(email);
        return rentals;
    }

    // GET: api/rental/user/{email}/active
    [HttpGet("user/{email}/active")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllActiveRentalsOfUser(string email)
    {
        _logger.LogInformation("Fetching active rentals for user {Email} at {Time}", email, DateTime.UtcNow);
        var rentals = await _rentalService.GetAllActiveRentalsOfUserAsync(email);
        return rentals;
    }

    // GET: api/rental/user/{email}/cancelled
    [HttpGet("user/{email}/cancelled")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCancelledRentalsOfUser(string email)
    {
        _logger.LogInformation("Fetching cancelled rentals for user {Email} at {Time}", email, DateTime.UtcNow);

        var rentals = await _rentalService.GetAllCancelledRentalsOfUserAsync(email);
        return rentals;
    }

    // GET: api/rental/user/{email}/pending
    [HttpGet("user/{email}/pending")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllPendingRentalsOfUser(string email)
    {
        _logger.LogInformation("Fetching pending rentals for user {Email} at {Time}", email, DateTime.UtcNow);

        var rentals = await _rentalService.GetAllPendingRentalsOfUserAsync(email);
        return rentals;
    }

    // GET: api/rental/user/{email}/completed
    [HttpGet("user/{email}/completed")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCompletedRentalsOfUser(string email)
    {
        _logger.LogInformation("Fetching completed rentals for user {Email} at {Time}", email, DateTime.UtcNow);

        var rentals = await _rentalService.GetAllCompletedRentalsOfUserAsync(email);
        return rentals;
    }

    // GET: api/rental
    [HttpGet]
    public async Task<ActionResult<List<RentalDTO>>> GetAllRentals()
    {
        _logger.LogInformation("Fetching all rentals stored at {Time}", DateTime.UtcNow);

        var rentals = await _rentalService.GetAllRentalsAsync();
        return rentals;
    }

    // GET: api/rental/active
    [HttpGet("active")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllActiveRentals()
    {
        _logger.LogInformation("Fetching all active rentals at {Time}", DateTime.UtcNow);

        var rentals = await _rentalService.GetAllActiveRentalsAsync();
        return rentals;
    }

    // GET: api/rental/cancelled
    [HttpGet("cancelled")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCancelledRentals()
    {
        _logger.LogInformation("Fetching all cancelled rentals at {Time}", DateTime.UtcNow);

        var rentals = await _rentalService.GetAllCancelledRentalsAsync();
        return rentals;
    }

    // GET: api/rental/pending
    [HttpGet("pending")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllPendingRentals()
    {
        _logger.LogInformation("Fetching all pending rentals at {Time}", DateTime.UtcNow);

        var rentals = await _rentalService.GetAllPendingRentalsAsync();
        return rentals;
    }

    // GET: api/rental/completed
    [HttpGet("completed")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCompletedRentals()
    {
        _logger.LogInformation("Fetching all completed rentals at {Time}", DateTime.UtcNow);

        var rentals = await _rentalService.GetAllCompletedRentalsAsync();
        return rentals;
    }

    // GET: api/rental/{id} 
    [HttpGet("{id}")]
    public async Task<ActionResult<RentalDTO>> GetRentalById(string id)
    {
        _logger.LogInformation("Fetching rental with ID {Id} at {Time}", id, DateTime.UtcNow);

        var rentals = await _rentalService.GetAllRentalsAsync();
        var rental = rentals.FirstOrDefault(r => r.IdRentalValue == id);
        if (rental == null)
        {
            _logger.LogWarning("Rental with ID {Id} not found at {Time}", id, DateTime.UtcNow);
            return NotFound("Rental not found.");
        }
        return rental;
    }
}