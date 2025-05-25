using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public interface IBookRepository : IRepository<Book, BookID>
{
}