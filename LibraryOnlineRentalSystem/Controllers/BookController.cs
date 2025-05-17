using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace LibraryOnlineRentalSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly BookService _bookService;


    public BookController(BookService bookService)
    {
        _bookService = bookService;
    }

    // GET: api/Book
    [HttpGet]
    public async Task<ActionResult<List<BookDTO>>> GetAllBooks()
    {
        var response = await _bookService.GetAllBooks();
        if (response == null) return NotFound();

        return response;
    }


    // GET: api/Book/{id}
    public async Task<ActionResult<BookDTO>> GetByIdAsync(string id)
    {
        var response = await _bookService.GetBookByID(id);
        if (response == null) return NotFound();
        return response;
    }
    
    // POST: api/Book
    [HttpPost]
    public async Task<ActionResult<BookDTO>> CreateBook (NewBookDTO BookToAddDto)
    {
        try
        {
            var bookToAdd = await _bookService.AddBook(BookToAddDto);
            return bookToAdd;
        }
        catch (BusinessRulesException ex)
        {
            return BadRequest(new {ex.Message});
        }
    }
    
    
}