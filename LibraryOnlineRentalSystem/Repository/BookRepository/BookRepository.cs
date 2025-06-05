using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Repository.Common;

namespace LibraryOnlineRentalSystem.Repository.BookRepository;

public class BookRepository : GeneralRepository<Book, BookID>,
    IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context) : base(context.Books)
    {
        _context = context;
    }

    public Book UpdateBookStock(string bookId, int currentAmountOfCopiesStock)
    {
        try
        {
            var book = context().SingleOrDefault(b => b.Id == new BookID(bookId));
            if (book == null)
                return null;

            book.UpdateStock(currentAmountOfCopiesStock);

            _context.SaveChanges();
            return book;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}