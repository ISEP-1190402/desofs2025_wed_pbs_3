namespace LibraryOnlineRentalSystem.Domain.Book;

public class Description
{
    public string BookDescription { get; private set; }


    public Description(string bookDescription)
    {
        if (string.IsNullOrEmpty(bookDescription))
        {
            throw new ArgumentException("Description cannot be null or empty");
        }

        this.BookDescription = bookDescription.Trim();
    }


    public string GetBookDescription()
    {
        return BookDescription;
    }


    public Description ValueOf(string bookDescription)
    {
        return new Description(bookDescription);
    }
    
    public override bool Equals(object? obj)
    {
        if (this == obj) return true;

        if (obj == null || obj.GetType() != GetType()) return false;

        var that = (Description) obj;

        return BookDescription.ToUpper().Equals(that.BookDescription.ToUpper());
    }

    public override string ToString()
    {
        return $"{BookDescription}";
    }
}