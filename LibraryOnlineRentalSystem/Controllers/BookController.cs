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
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<BookController> _logger;

    public BookController(BookService bookService, IBookRepository bookRepository, ILogger<BookController> logger)
    {
        _bookService = bookService;
        _bookRepository = bookRepository;
        _logger = logger;
    }

    // GET: api/Book working
    // Access: Public
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<BookDTO>>> GetAllBooks()
    {
        _logger.LogInformation("=== GetAllBooks Start ===");
        _logger.LogInformation("Book list requested at {Time}", DateTime.UtcNow);

        try
        {
            var books = await _bookService.GetAllBooks();
            _logger.LogInformation("Returned {Count} books at {Time}", books.Count, DateTime.UtcNow);
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving books");
            return StatusCode(500, new { message = "An error occurred while retrieving books" });
        }
    }

    // GET: api/Book/search/name/{name} working
    // Access: Public
    [HttpGet("search/name/{name}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<BookDTO>>> GetBooksByName(string name)
    {
        _logger.LogInformation("Searching books by name: {Name}", name);

        if (string.IsNullOrWhiteSpace(name))
            return Ok(new List<BookDTO>());

        try
        {
            var books = await _bookRepository.GetBooksByNameAsync(name);
            var bookDtos = books.ConvertAll(b => b.toDTO());
            _logger.LogInformation("Found {Count} books matching name: {Name}", bookDtos.Count, name);
            return Ok(bookDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching books by name: {Name}", name);
            return StatusCode(500, new { message = "An error occurred while searching books by name" });
        }
    }

    // GET: api/Book/search/isbn/{isbn} working
    // Access: Public
    [HttpGet("search/isbn/{isbn}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<BookDTO>>> GetBooksByIsbn(string isbn)
    {
        _logger.LogInformation("Searching books by ISBN: {Isbn}", isbn);

        if (string.IsNullOrWhiteSpace(isbn))
            return Ok(new List<BookDTO>());

        try
        {
            var book = await _bookRepository.GetBookByIsbnAsync(isbn);
            var books = book != null ? new List<BookDTO> { book.toDTO() } : new List<BookDTO>();
            _logger.LogInformation("Found {Count} books with ISBN: {Isbn}", books.Count, isbn);
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching books by ISBN: {Isbn}", isbn);
            return StatusCode(500, new { message = "An error occurred while searching books by ISBN" });
        }
    }

    // GET: api/Book/search/author/{author} working
    // Access: Public
    [HttpGet("search/author/{author}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<BookDTO>>> GetBooksByAuthor(string author)
    {
        _logger.LogInformation("Searching books by author: {Author}", author);

        if (string.IsNullOrWhiteSpace(author))
            return Ok(new List<BookDTO>());

        try
        {
            var books = await _bookRepository.GetBooksByAuthorAsync(author);
            var bookDtos = books.ConvertAll(b => b.toDTO());
            _logger.LogInformation("Found {Count} books by author: {Author}", bookDtos.Count, author);
            return Ok(bookDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching books by author: {Author}", author);
            return StatusCode(500, new { message = "An error occurred while searching books by author" });
        }
    }


    // GET: api/Book/{id} working
    // Access: Public
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin, LibraryManager")]
    public async Task<ActionResult<BookDTO>> GetByIdAsync(string id)
    {
        _logger.LogInformation("=== GetBookById Start ===");
        _logger.LogInformation("Book ID {Id} requested at {Time}", id, DateTime.UtcNow);

        try
        {
            var book = await _bookService.GetBookByID(id);
            if (book == null)
            {
                _logger.LogWarning("Book with ID {Id} not found", id);
                return NotFound(new { message = "Book not found" });
            }

            _logger.LogInformation("Book '{Name}' (ID: {Id}) returned", book.Name, id);
            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the book" });
        }
    }

    // POST: api/Book working
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

    // PUT: api/Book/updatestock/{id} working
    // Access: LibraryManager
    [HttpPut("updatestock/{id}")]
    [Authorize(Roles = "LibraryManager")]
    public async Task<ActionResult<BookDTO>> UpdateStock(string id, [FromBody] BookStockDTO bookStockUpdateDto)
    {
        _logger.LogInformation("=== UpdateStock Start ===");
        var currentUser = await GetCurrentUserInfo();

        _logger.LogInformation("Stock update for book ID {Id} requested by {Username}",
            id, currentUser.Username);

        try
        {
            var updatedBook = await _bookService.UpdateStock(id, bookStockUpdateDto);
            if (updatedBook == null)
            {
                _logger.LogWarning("Failed to update stock for book ID {Id} - Book not found", id);
                return NotFound(new { message = "Book not found" });
            }

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