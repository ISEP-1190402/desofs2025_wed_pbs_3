using LibraryOnlineRentalSystem.Repository.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly WorkUnity _workUnity;

    public BookService(IBookRepository bookRepository, WorkUnity workUnity)
    {
        _bookRepository = bookRepository;
        _workUnity = workUnity;
    }

    public async Task<List<BookDTO>> GetAllBooks()
    {
        var list = await _bookRepository.GetAllAsync();
        var listDTO = list.ConvertAll(book =>
            book.toDTO());
        return listDTO;
    }

    public async Task<BookDTO> GetBookByID(string id)
    {
        var book = await _bookRepository.GetByIdAsync(new BookID(id));
        var bookDTO = book.toDTO();
        return bookDTO;
    }
}