using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Rentals;
using LibraryOnlineRentalSystem.Repository.Common;
using Microsoft.AspNetCore.Mvc;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IWorkUnity _workUnity;

    public BookService(IBookRepository bookRepository, IWorkUnity workUnity)
    {
        _bookRepository = bookRepository;
        _workUnity = workUnity;
    }

    public async Task<List<BookDTO>> GetAllBooks()
    {
        var list = await _bookRepository.GetAllAsync();
        var listDto = list.ConvertAll(book =>
            book.toDTO());
        return listDto;
    }

    public async Task<BookDTO> GetBookByID(string id)
    {
        var book = await _bookRepository.GetByIdAsync(new BookID(id));
        var bookDTO = book.toDTO();
        return bookDTO;
    }

    public async Task<BookDTO> AddBook(NewBookDTO bookToAddDto)
    {
 
        var id = (await _bookRepository.GetAllAsync()).Count + 1 + "";

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

        return addedBook.toDTO();
    }


    public async Task<BookDTO> UpdateStock(string id, BookStockDTO bookStockUpdateDto)
    {
        var book = _bookRepository.UpdateBookStock(id, bookStockUpdateDto.AmountOfCopies);
        if (book == null)
            return null;
            
        await _workUnity.CommitAsync();
        return book.toDTO();
    }

    public bool BookExists(string reservedBookId)
    {
        var book = _bookRepository.GetByIdAsync(new BookID(reservedBookId)).Result;
        return book != null;
    }

    public int GetAmmountOfBooks(BookID bookId)
    {
        return _bookRepository.GetAmmountOfBooks(bookId);
    }
    
    public async Task<List<BookDTO>> GetBooksByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new List<BookDTO>();
            
        var books = await _bookRepository.GetBooksByNameAsync(name);
        return books.ConvertAll(book => book.toDTO());
    }
    
    public async Task<List<BookDTO>> GetBooksByIsbnAsync(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return new List<BookDTO>();
            
        var book = await _bookRepository.GetBookByIsbnAsync(isbn);
        return book != null ? new List<BookDTO> { book.toDTO() } : new List<BookDTO>();
    }
    
    public async Task<List<BookDTO>> GetBooksByAuthorAsync(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            return new List<BookDTO>();
            
        var books = await _bookRepository.GetBooksByAuthorAsync(author);
        return books.ConvertAll(book => book.toDTO());
    }
    
    public async Task<List<BookDTO>> GetBooksByPublisherAsync(string publisher)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            return new List<BookDTO>();
            
        var books = await _bookRepository.GetBooksByPublisherAsync(publisher);
        return books.ConvertAll(book => book.toDTO());
    }
    
    public async Task<List<BookDTO>> GetBooksByCategoryAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return new List<BookDTO>();
            
        var books = await _bookRepository.GetBooksByCategoryAsync(category);
        return books.ConvertAll(book => book.toDTO());
    }
}