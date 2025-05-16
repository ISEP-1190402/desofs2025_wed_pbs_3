namespace LibraryOnlineRentalSystem.Domain.Book;

public class ISBN : IValueObject
{
    public ISBN(string isbn)
    {
        if (string.IsNullOrEmpty(isbn)) throw new BusinessRulesException("ISBN cannot be null or empty");

        if (!IsISBNValid(isbn)) throw new BusinessRulesException("Invalid ISBN");

        BookISBN = isbn.Trim();
    }

    public string BookISBN { get; }

    public override int GetHashCode()
    {
        return BookISBN.GetHashCode();
    }


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