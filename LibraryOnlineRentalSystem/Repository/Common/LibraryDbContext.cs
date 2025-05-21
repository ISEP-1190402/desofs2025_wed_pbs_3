using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Role;
using LibraryOnlineRentalSystem.Repository.BookRepository;
using LibraryOnlineRentalSystem.Repository.UserRepository;
using LibraryOnlineRentalSystem.Repository.RoleRepository;
using Microsoft.EntityFrameworkCore;

namespace LibraryOnlineRentalSystem.Repository.Common;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ConfigBookEntityType());
        modelBuilder.ApplyConfiguration(new ConfigUserEntityType());
        modelBuilder.ApplyConfiguration(new ConfigRoleEntityType());

        // Seed default roles
        modelBuilder.Entity<Role>().HasData(
            new Role("Admin", "Administrator with full access"),
            new Role("LibraryManager", "Library manager with book management access"),
            new Role("User", "Regular user with basic access")
        );
    }
}