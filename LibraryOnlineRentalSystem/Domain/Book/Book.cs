using System.ComponentModel.Design;
using System.Text.Json.Serialization;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class Book : Entity<BookID>, IAggregateRoot
{
    public Book()
    {
    }

    [JsonConstructor]
    public Book(string id, string name, int amountOfCopies, string author, string category, string description,
        string isbn, string publisher)
    {
        Id = new BookID(id);
        Name = new BookName(name);
        AmountOfCopies = new AmountOfCopies(amountOfCopies);
        Author = new Author(author);
        Category = new Category(category);
        Description = new Description(description);
        Isbn = new ISBN(isbn);
        Publisher = new Publisher(publisher);
        Active = true;
    }

    public BookID Id { get; private set; }
    public BookName Name { get; private set; }
    public AmountOfCopies AmountOfCopies { get; private set; }
    public Author Author { get; private set; }
    public Category Category { get; private set; }
    public Description Description { get; private set; }
    public ISBN Isbn { get; private set; }
    public Publisher Publisher { get; private set; }
    public bool Active { get; private set; }

    public bool isBookDeleted()
    {
        return !Active;
    }

    public void deleteBook()
    {
        Active = false;
    }

    public BookDTO toDTO()
    {
        return new BookDTO(Id.Value, Name.GetValue(), AmountOfCopies.GetBookAmountOfCopies(), Author.GetBookAuthor(),
            Category.GetBookCategoryName(), Description.GetBookDescription(), Isbn.GetISBN(),
            Publisher.GetBookPublisher());
    }

    public void UpdateStock(int currentAmountOfCopiesStock)
    {
        this.AmountOfCopies = new AmountOfCopies(currentAmountOfCopiesStock);
    }
}