using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Repository.BookRepository;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Repository.UserRepository;
using Microsoft.EntityFrameworkCore;

namespace LibraryOnlineRentalSystem.Repository.Common;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Domain.User.User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ConfigBookEntityType());
    }
}