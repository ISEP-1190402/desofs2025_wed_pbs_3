using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public interface IBookRepository : IRepository<Book, BookID>
{
    Book UpdateBookStock(string id, int bookStockUpdateDto);
    int GetAmmountOfBooks(BookID bookId);
    Task<bool> BookWithIsbnExistsAsync(string isbn);
    Task<Book> GetBookByIsbnAsync(string isbn);
    
    // Search methods
    Task<List<Book>> GetBooksByNameAsync(string name);
    Task<List<Book>> GetBooksByAuthorAsync(string author);
    Task<List<Book>> GetBooksByPublisherAsync(string publisher);
    Task<List<Book>> GetBooksByCategoryAsync(string category);
}