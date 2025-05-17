using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Repository.Common;

public class BookRepository : GeneralRepository<Book, BookID>,
    IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context.Books)
    {
    }
}