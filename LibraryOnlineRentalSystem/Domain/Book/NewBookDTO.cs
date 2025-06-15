namespace LibraryOnlineRentalSystem.Domain.Book;

public class NewBookDTO
{
    public string Name { get; private set; }
    public int AmountOfCopies { get; private set; }
    public string Author { get; private set; }
    public string Category { get; private set; }
    public string Description { get; private set; }
    public string Isbn { get; private set; }
    public string Publisher { get; private set; }

    public NewBookDTO(string name, int amountOfCopies, string author, string category, string description, string isbn, string publisher)
    {
        Name = name;
        AmountOfCopies = amountOfCopies;
        Author = author;
        Category = category;
        Description = description;
        Isbn = isbn;
        Publisher = publisher;
    }
}