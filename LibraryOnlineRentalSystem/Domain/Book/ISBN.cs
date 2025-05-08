namespace LibraryOnlineRentalSystem.Domain.Book;

public class ISBN
{
    public string BookISBN { get; private set; }


    public ISBN(string isbn)
    {
        if (string.IsNullOrEmpty(isbn))
        {
            throw new ArgumentException("ISBN cannot be null or empty");
        }

        this.BookISBN = isbn.Trim();
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

        var that = (ISBN) obj;

        return BookISBN.ToUpper().Equals(that.BookISBN.ToUpper());
    }

    public override string ToString()
    {
        return $"{BookISBN}";
    }
}