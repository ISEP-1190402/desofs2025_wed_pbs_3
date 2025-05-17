using LibraryOnlineRentalSystem.Domain.Book;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryOnlineRentalSystem.Repository.BookRepository;

public class ConfigBookEntityType : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.OwnsOne(b => b.AmountOfCopies,
            ammountOfCopies => { ammountOfCopies.Property("BookAmountOfCopies").IsRequired(); });
        builder.OwnsOne(b => b.Author,
            author => { author.Property("BookAuthor").IsRequired(); });
        builder.OwnsOne(b => b.Category,
            category => { category.Property("BookCategoryName").IsRequired(); });
        builder.OwnsOne(b => b.Description,
            description => { description.Property("BookDescription").IsRequired(); });
        builder.OwnsOne(b => b.Isbn,
            isbn => { isbn.Property("BookISBN").IsRequired(); });
        builder.OwnsOne(b => b.Publisher,
            publisher => { publisher.Property("PublisherName").IsRequired(); });
    }
}