using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Rentals;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LibraryOnlineRentalSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalController
{
    private readonly RentalService _rentalService;
    private readonly UserService _userService;
    private readonly BookService _bookService;

    public RentalController(RentalService rentalService, UserService userService, BookService bookService)
    {
        _rentalService = rentalService;
        _userService = userService;
        _bookService = bookService;
    }

    // POST: api/rental
    [HttpPost]
    public async Task<ActionResult<RentalDTO>> CreateRental(CreatedRentalDTO dto)
    {
        if (!_userService.UserExists(dto.UserEmail) || !_bookService.BookExists(dto.ReservedBookId))
        {
            return null;
        }

        DateTime start;
        DateTime end;

        if (!DateTime.TryParse(dto.StartDate, out start))
        {
            throw new ArgumentException("Invalid start date");
        }

        if (!DateTime.TryParse(dto.EndDate, out end))
        {
            throw new ArgumentException("Invalid end date.");
        }

        if (end < start)
        {
            throw new ArgumentException("The end date cannot be earlier than the start date.");
        }


        // Check Stock


        var rental = await _rentalService.CreateARentalAsync(dto);
        if (rental != null)
        {
            FilePrint.RentalFilePrint(rental.ToString());
        }

        return rental;
    }

    // PUT: api/rental/cancel/{id}
    [HttpPut("cancel/{id}")]
    public async Task<ActionResult<RentalDTO>> CancelRental(string id)
    {
        var rental = await _rentalService.CancelARental(id);
        if (rental == null) return null;
        return rental;
    }

    // GET: api/rental/user/{email}
    [HttpGet("user/{email}")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllRentalsOfUser(string email)
    {
        var rentals = await _rentalService.GetAllRentalsOfUserAsync(email);
        return rentals;
    }

    // GET: api/rental/user/{email}/active
    [HttpGet("user/{email}/active")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllActiveRentalsOfUser(string email)
    {
        var rentals = await _rentalService.GetAllActiveRentalsOfUserAsync(email);
        return rentals;
    }

    // GET: api/rental/user/{email}/cancelled
    [HttpGet("user/{email}/cancelled")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCancelledRentalsOfUser(string email)
    {
        var rentals = await _rentalService.GetAllCancelledRentalsOfUserAsync(email);
        return rentals;
    }

    // GET: api/rental/user/{email}/pending
    [HttpGet("user/{email}/pending")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllPendingRentalsOfUser(string email)
    {
        var rentals = await _rentalService.GetAllPendingRentalsOfUserAsync(email);
        return rentals;
    }

    // GET: api/rental/user/{email}/completed
    [HttpGet("user/{email}/completed")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCompletedRentalsOfUser(string email)
    {
        var rentals = await _rentalService.GetAllCompletedRentalsOfUserAsync(email);
        return rentals;
    }

    // GET: api/rental
    [HttpGet]
    public async Task<ActionResult<List<RentalDTO>>> GetAllRentals()
    {
        var rentals = await _rentalService.GetAllRentalsAsync();
        return rentals;
    }

    // GET: api/rental/active
    [HttpGet("active")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllActiveRentals()
    {
        var rentals = await _rentalService.GetAllActiveRentalsAsync();
        return rentals;
    }

    // GET: api/rental/cancelled
    [HttpGet("cancelled")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCancelledRentals()
    {
        var rentals = await _rentalService.GetAllCancelledRentalsAsync();
        return rentals;
    }

    // GET: api/rental/pending
    [HttpGet("pending")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllPendingRentals()
    {
        var rentals = await _rentalService.GetAllPendingRentalsAsync();
        return rentals;
    }

    // GET: api/rental/completed
    [HttpGet("completed")]
    public async Task<ActionResult<List<RentalDTO>>> GetAllCompletedRentals()
    {
        var rentals = await _rentalService.GetAllCompletedRentalsAsync();
        return rentals;
    }

    // GET: api/rental/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RentalDTO>> GetRentalById(string id)
    {
        var rentals = await _rentalService.GetAllRentalsAsync();
        var rental = rentals.FirstOrDefault(r => r.IdRentalValue == id);
        if (rental == null) return null;
        return rental;
    }
}