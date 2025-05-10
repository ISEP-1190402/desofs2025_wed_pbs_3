namespace LibraryOnlineRentalSystem.Domain.Book;

public class ISBN
{
    public ISBN(string isbn)
    {
        if (string.IsNullOrEmpty(isbn)) throw new ArgumentException("ISBN cannot be null or empty");

        if (!IsISBNValid(isbn)) throw new ArgumentException("Invalid ISBN");

        BookISBN = isbn.Trim();
    }

    public string BookISBN { get; }


    public string GetBookDescription()
    {
        return BookISBN;
    }


    public ISBN ValueOf(string isbn)
    {
        return new ISBN(isbn);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;

        if (obj == null || obj.GetType() != GetType()) return false;

        var that = (ISBN)obj;

        return BookISBN.ToUpper().Equals(that.BookISBN.ToUpper());
    }

    public override string ToString()
    {
        return $"{BookISBN}";
    }

    public static bool IsISBNValid(string isbnToTest)
    {
        // TODO - ISBN VALIDATION METHOD
        return false;
    }
}