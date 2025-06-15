using System;
using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Repository.Common;
using Microsoft.EntityFrameworkCore;

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

    public int GetAmmountOfBooks(BookID bookId)
    {
        try
        {
            return context().SingleOrDefault(b => b.Id == bookId).AmountOfCopies.BookAmountOfCopies;
        }
        catch (NullReferenceException ex)
        {
            return 0;
        }
    }

    public async Task<bool> BookWithIsbnExistsAsync(string isbn)
    {
        // First try to find an exact match
        var exists = await context()
            .AnyAsync(b => b.Isbn.BookISBN == isbn);
            
        if (exists)
            return true;
            
        // If no exact match, try case-insensitive search in memory as fallback
        var existingIsbns = await context()
            .Select(b => b.Isbn.BookISBN)
            .ToListAsync();
            
        return existingIsbns.Any(b => string.Equals(b, isbn, StringComparison.OrdinalIgnoreCase));
    }
    
    public async Task<Book> GetBookByIsbnAsync(string isbn)
    {
        // First try to find an exact match
        var book = await context()
            .FirstOrDefaultAsync(b => b.Isbn.BookISBN == isbn);
            
        if (book != null)
            return book;
            
        // If no exact match, try case-insensitive search in memory as fallback
        var books = await context().ToListAsync();
        return books.FirstOrDefault(b => 
            string.Equals(b.Isbn.BookISBN, isbn, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<Book>> GetBooksByNameAsync(string name)
    {
        // First try to find exact or partial matches in the database
        var books = await context()
            .Where(b => EF.Functions.Like(b.Name.NameBook, $"%{name}%"))
            .ToListAsync();
            
        // If no matches found, try case-insensitive search in memory
        if (!books.Any())
        {
            var allBooks = await context().ToListAsync();
            books = allBooks
                .Where(b => b.Name.NameBook.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
        
        return books;
    }
    
    public async Task<List<Book>> GetBooksByAuthorAsync(string author)
    {
        // First try to find exact or partial matches in the database
        var books = await context()
            .Where(b => EF.Functions.Like(b.Author.BookAuthor, $"%{author}%"))
            .ToListAsync();
            
        // If no matches found, try case-insensitive search in memory
        if (!books.Any())
        {
            var allBooks = await context().ToListAsync();
            books = allBooks
                .Where(b => b.Author.BookAuthor.IndexOf(author, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
        
        return books;
    }
    
    public async Task<List<Book>> GetBooksByPublisherAsync(string publisher)
    {
        // First try to find exact or partial matches in the database
        var books = await context()
            .Where(b => EF.Functions.Like(b.Publisher.PublisherName, $"%{publisher}%"))
            .ToListAsync();
            
        // If no matches found, try case-insensitive search in memory
        if (!books.Any())
        {
            var allBooks = await context().ToListAsync();
            books = allBooks
                .Where(b => b.Publisher.PublisherName.IndexOf(publisher, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
        
        return books;
    }
    
    public async Task<List<Book>> GetBooksByCategoryAsync(string category)
    {
        // First try to find exact or partial matches in the database
        var books = await context()
            .Where(b => EF.Functions.Like(b.Category.BookCategoryName, $"%{category}%"))
            .ToListAsync();
            
        // If no matches found, try case-insensitive search in memory
        if (!books.Any())
        {
            var allBooks = await context().ToListAsync();
            books = allBooks
                .Where(b => b.Category.BookCategoryName.IndexOf(category, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
        
        return books;
    }
}