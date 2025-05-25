using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Repository.BookRepository;
using Microsoft.EntityFrameworkCore;

namespace LibraryOnlineRentalSystem.Repository.Common;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ConfigBookEntityType());
    }
}