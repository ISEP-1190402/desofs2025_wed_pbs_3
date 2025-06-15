using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Rentals;
using LibraryOnlineRentalSystem.Repository.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IWorkUnity _workUnity;
    private readonly ILogger<BookService> _logger;

    public BookService(IBookRepository bookRepository, IWorkUnity workUnity, ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _workUnity = workUnity;
        _logger = logger;
    }

    public async Task<List<BookDTO>> GetAllBooks()
    {
        _logger.LogInformation("Fetching all books from repository.");

        var list = await _bookRepository.GetAllAsync();
        var listDto = list.ConvertAll(book =>
            book.toDTO());
        _logger.LogInformation("Fetched {Count} books.", listDto.Count);

        return listDto;
    }

    public async Task<BookDTO> GetBookByID(string id)
    {
        _logger.LogInformation("Fetching book with ID: {BookID}", id);

        var book = await _bookRepository.GetByIdAsync(new BookID(id));
        var bookDTO = book.toDTO();

        if (book == null)
        {
            _logger.LogWarning("Book with ID {BookID} not found.", id);
            return null;
        }
        return bookDTO;
    }

    public async Task<BookDTO> AddBook(NewBookDTO bookToAddDto)
    {

        var id = (await _bookRepository.GetAllAsync()).Count + 1 + "";

        _logger.LogInformation("Adding new book with generated ID: {BookID}", id);

        var addedBook = new Book(
            id: id,
            name: bookToAddDto.Name,
            amountOfCopies: bookToAddDto.AmountOfCopies,
            author: bookToAddDto.Author,
            category: bookToAddDto.Category,
            description: bookToAddDto.Description,
            isbn: bookToAddDto.Isbn,
            publisher: bookToAddDto.Publisher);

        await _bookRepository.AddAsync(addedBook);
        await _workUnity.CommitAsync();

        _logger.LogInformation("Added new book: {BookName} by {Author}", addedBook.Name, addedBook.Author);

        return addedBook.toDTO();
    }


    public async Task<BookDTO> UpdateStock(string id, BookStockDTO bookStockUpdateDto)
    {
        _logger.LogInformation("Updating stock for book ID: {BookID}", id);

        var book = _bookRepository.UpdateBookStock(id, bookStockUpdateDto.AmountOfCopies);
        if (book == null)
        {
            _logger.LogWarning("Book with ID {BookID} not found for stock update.", id);
            return null;
        }

        await _workUnity.CommitAsync();
        _logger.LogInformation("Stock updated to {Copies} for book ID: {BookID}", bookStockUpdateDto.AmountOfCopies, id);

        return book.toDTO();
    }

    public bool BookExists(string reservedBookId)
    {
        _logger.LogInformation("Checking if book exists with ID: {BookID}", reservedBookId);

        var book = _bookRepository.GetByIdAsync(new BookID(reservedBookId)).Result;
        var exists = book != null;
        _logger.LogInformation("Book exists: {Exists}", exists);

        return exists;
    }

    public int GetAmmountOfBooks(BookID bookId)
    {
        _logger.LogInformation("Getting amount of copies for BookID: {BookID}", bookId);

        return _bookRepository.GetAmmountOfBooks(bookId);
    }

    public async Task<List<BookDTO>> GetBooksByNameAsync(string name)
    {
        _logger.LogInformation("Searching books by name: {Name}", name);

        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("Search by name called with empty or null value.");
            return new List<BookDTO>();
        }

        var books = await _bookRepository.GetBooksByNameAsync(name);
        return books.ConvertAll(book => book.toDTO());
    }

    public async Task<List<BookDTO>> GetBooksByIsbnAsync(string isbn)
    {
        _logger.LogInformation("Searching books by ISBN: {ISBN}", isbn);

        if (string.IsNullOrWhiteSpace(isbn))
        {
            _logger.LogWarning("Search by ISBN called with empty or null value.");
            return new List<BookDTO>();
        }

        var book = await _bookRepository.GetBookByIsbnAsync(isbn);
        return book != null ? new List<BookDTO> { book.toDTO() } : new List<BookDTO>();
    }

    public async Task<List<BookDTO>> GetBooksByAuthorAsync(string author)
    {
        _logger.LogInformation("Searching books by author: {Author}", author);
        if (string.IsNullOrWhiteSpace(author))
        {
            _logger.LogWarning("Search by author called with empty or null value.");
            return new List<BookDTO>();
        }

        var books = await _bookRepository.GetBooksByAuthorAsync(author);
        return books.ConvertAll(book => book.toDTO());
    }

    public async Task<List<BookDTO>> GetBooksByPublisherAsync(string publisher)
    {
        _logger.LogInformation("Searching books by publisher: {Publisher}", publisher);
        if (string.IsNullOrWhiteSpace(publisher))
        {
            _logger.LogWarning("Search by publisher called with empty or null value.");
            return new List<BookDTO>();
        }

        var books = await _bookRepository.GetBooksByPublisherAsync(publisher);
        return books.ConvertAll(book => book.toDTO());
    }

    public async Task<List<BookDTO>> GetBooksByCategoryAsync(string category)
    {
        _logger.LogInformation("Searching books by category: {Category}", category);
        if (string.IsNullOrWhiteSpace(category))
        {
            _logger.LogWarning("Search by category called with empty or null value.");
            return new List<BookDTO>();
        }

        var books = await _bookRepository.GetBooksByCategoryAsync(category);
        return books.ConvertAll(book => book.toDTO());
    }
}