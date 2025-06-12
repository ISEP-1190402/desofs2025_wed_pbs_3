using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LibraryOnlineRentalSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly BookService _bookService;
    private readonly ILogger<BookController> _logger;


    public BookController(BookService bookService, ILogger<BookController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    // GET: api/Book
    [HttpGet]
    public async Task<ActionResult<List<BookDTO>>> GetAllBooks()
    {
        _logger.LogInformation("Fetching all books at {Time}", DateTime.UtcNow);

        var response = await _bookService.GetAllBooks();
        if (response == null)
        {
            _logger.LogWarning("No books found at {Time}", DateTime.UtcNow);
            return NotFound();
        }

        return response;
    }


    // GET: api/Book/{id}
    [HttpGet("{id}")]

    public async Task<ActionResult<BookDTO>> GetByIdAsync(string id)
    {
        _logger.LogInformation("Fetching book with ID {Id} at {Time}", id, DateTime.UtcNow);

        var response = await _bookService.GetBookByID(id);
        if (response == null)
        {
            _logger.LogWarning("Book with ID {Id} not found at {Time}", id, DateTime.UtcNow);
            return NotFound();
        }
        return response;
    }

    // POST: api/Book
    [HttpPost]
    public async Task<ActionResult<BookDTO>> CreateBook(NewBookDTO BookToAddDto)
    {
        try
        {
            var bookToAdd = await _bookService.AddBook(BookToAddDto);
            _logger.LogInformation("New book created: {Author} at {Time}", BookToAddDto.Author, DateTime.UtcNow);

            return bookToAdd;
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning("Failed to create book at {Time}: {Message}", DateTime.UtcNow, ex.Message);

            return BadRequest(new { ex.Message });
        }
    }

    // PUT: api/Book
    [HttpPut("updatestock/{id}")]

    public async Task<ActionResult<BookDTO>> UpdateStock(string id, BookStockDTO bookSotckUpdateDTO)
    {
        try
        {
            var quantityUpdate = await _bookService.UpdateStock(id, bookSotckUpdateDTO);
            _logger.LogInformation("Stock updated for book ID {Id} at {Time}, New quantity: {Qty}", id, DateTime.UtcNow, quantityUpdate);

            return quantityUpdate;
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning("Failed to update stock for book ID {Id} at {Time}: {Message}", id, DateTime.UtcNow, ex.Message);

            return BadRequest(new { ex.Message });
        }
    }
}