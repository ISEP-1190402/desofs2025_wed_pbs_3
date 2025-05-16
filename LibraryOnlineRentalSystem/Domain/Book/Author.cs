namespace LibraryOnlineRentalSystem.Domain.Book;

public class Author
{
    public Author(string author)
    {
        if (string.IsNullOrEmpty(author)) throw new ArgumentException("Author cannot be null or empty");

        BookAuthor = author.Trim();
    }

    public string BookAuthor { get; }


    public string GetBookAuthor()
    {
        return BookAuthor;
    }


    public Author ValueOf(string author)
    {
        return new Author(author);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;

        if (obj == null || obj.GetType() != GetType()) return false;

        var that = (Author)obj;

        return BookAuthor.ToUpper().Equals(that.BookAuthor.ToUpper());
    }

    public override string ToString()
    {
        return $"{BookAuthor}";
    }

    public override int GetHashCode()
    {
        return BookAuthor.GetHashCode();
    }
}