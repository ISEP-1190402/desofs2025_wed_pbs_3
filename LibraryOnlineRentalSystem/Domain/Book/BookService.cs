using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Repository.Common;

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

    public async Task<BookDTO> AddBook(NewBookDTO bookToAddDto)
    {
        var id=_bookRepository.GetAllAsync().Result.Count + 1+"";

        var addedBook= new Book(id,bookToAddDto.AmountOfCopies,bookToAddDto.Author,bookToAddDto.Category,
            bookToAddDto.Description,bookToAddDto.Isbn,bookToAddDto.Publisher);

            
        await _bookRepository.AddAsync(addedBook);

        await _workUnity.CommitAsync();

        return addedBook.toDTO();
    }
    
    
    
    
    
}