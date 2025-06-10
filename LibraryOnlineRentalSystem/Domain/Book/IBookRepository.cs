using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public interface IBookRepository : IRepository<Book, BookID>
{
    Book  UpdateBookStock(string id, int bookStockUpdateDto);
    int GetAmmountOfBooks(BookID bookId);
}