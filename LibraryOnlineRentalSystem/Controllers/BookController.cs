using System.Security.Claims;
using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using Microsoft.AspNetCore.Authorization;
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
    // Access: Public
    [HttpGet]
    public async Task<ActionResult<List<BookDTO>>> GetAllBooks()
    {
        _logger.LogInformation("=== GetAllBooks Start ===");
        var currentUser = await GetCurrentUserInfo();
        
        _logger.LogInformation("Book list requested by {Username} at {Time}", 
            currentUser.Username, DateTime.UtcNow);

        try
        {
            var books = await _bookService.GetAllBooks();
            _logger.LogInformation("Returned {Count} books to {Username} at {Time}", 
                books.Count, currentUser.Username, DateTime.UtcNow);
                
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving books for {Username}", currentUser.Username);
            return StatusCode(500, new { message = "An error occurred while retrieving books" });
        }
    }


    // GET: api/Book/{id}
    // Access: Public
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDTO>> GetByIdAsync(string id)
    {
        _logger.LogInformation("=== GetBookById Start ===");
        var currentUser = await GetCurrentUserInfo();
        
        _logger.LogInformation("Book ID {Id} requested by {Username} at {Time}", 
            id, currentUser.Username, DateTime.UtcNow);

        try
        {
            var book = await _bookService.GetBookByID(id);
            if (book == null)
            {
                _logger.LogWarning("Book with ID {Id} not found - Requested by {Username}", 
                    id, currentUser.Username);
                return NotFound(new { message = "Book not found" });
            }
            
            _logger.LogInformation("Book '{Title}' (ID: {Id}) returned to {Username}", 
                book.Name, id, currentUser.Username);
                
            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book ID {Id} for {Username}", 
                id, currentUser.Username);
            return StatusCode(500, new { message = "An error occurred while retrieving the book" });
        }
    }

    // POST: api/Book
    // Access: LibraryManager
    [HttpPost]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult> CreateBook(NewBookDTO bookToAddDto)
    {
        _logger.LogInformation("=== CreateBook Start ===");
        var currentUser = await GetCurrentUserInfo();
        
        _logger.LogInformation("New book creation requested by {Username}: {Title} by {Author}", 
            currentUser.Username, bookToAddDto.Name, bookToAddDto.Author);
            
        try
        {
            await _bookService.AddBook(bookToAddDto);
            _logger.LogInformation("Book '{Title}' created by {Username} at {Time}", 
                bookToAddDto.Name, currentUser.Username, DateTime.UtcNow);
                
            return Ok(new { message = "Book created successfully" });
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning("Business rule violation by {Username}: {Message}", 
                currentUser.Username, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating book by {Username}", currentUser.Username);
            return StatusCode(500, new { message = "An error occurred while creating the book" });
        }
    }

    // PUT: api/Book/updatestock/{id}
    // Access: LibraryManager
    [HttpPut("updatestock/{id}")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<BookDTO>> UpdateStock(string id, BookStockDTO bookStockUpdateDto)
    {
        _logger.LogInformation("=== UpdateStock Start ===");
        var currentUser = await GetCurrentUserInfo();
        
        _logger.LogInformation("Stock update for book ID {Id} requested by {Username}", 
            id, currentUser.Username);
            
        try
        {
            var result = await _bookService.UpdateStock(id, bookStockUpdateDto);
            if (result.Value == null)
            {
                _logger.LogWarning("Failed to update stock for book ID {Id} - Book not found", id);
                return NotFound(new { message = "Book not found" });
            }
            
            var updatedBook = result.Value;
            _logger.LogInformation("Stock updated for book '{Name}' (ID: {Id}) by {Username}. New amount of copies: {AmountOfCopies}", 
                updatedBook.Name, id, currentUser.Username, updatedBook.AmountOfCopies);

            return Ok(updatedBook);
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning("Stock update failed for book ID {Id} by {Username}: {Message}", 
                id, currentUser.Username, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for book ID {Id} by {Username}", 
                id, currentUser.Username);
            return StatusCode(500, new { message = "An error occurred while updating the book stock" });
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
}