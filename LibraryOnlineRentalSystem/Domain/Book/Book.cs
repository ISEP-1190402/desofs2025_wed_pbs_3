using System.Text.Json.Serialization;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class Book : Entity<BookID>, IAggregateRoot
{
    public Book()
    {
    }

    [JsonConstructor]
    public Book(string id, int amountOfCopies, string author, string category, string description,
        string isbn, string publisher)
    {
        Id = new BookID(id);
        AmountOfCopies = new AmountOfCopies(amountOfCopies);
        Author = new Author(author);
        Category = new Category(category);
        Description = new Description(description);
        Isbn = new ISBN(isbn);
        Publisher = new Publisher(publisher);
    }

    public BookID Id { get; private set; }
    public AmountOfCopies AmountOfCopies { get; private set; }
    public Author Author { get; private set; }
    public Category Category { get; private set; }
    public Description Description { get; private set; }
    public ISBN Isbn { get; private set; }
    public Publisher Publisher { get; private set; }
}