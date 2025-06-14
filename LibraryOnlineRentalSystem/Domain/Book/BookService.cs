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
        // ISBN validation is now handled by the ISBN class constructor
        // which will throw a BusinessRulesException if the ISBN is invalid or already exists
        
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


    public async Task<ActionResult<BookDTO>> UpdateStock(string id, BookStockDTO bookStockUpdateDto)
    {
        var book=  _bookRepository.UpdateBookStock(id,bookStockUpdateDto.AmountOfCopies);
        
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
}