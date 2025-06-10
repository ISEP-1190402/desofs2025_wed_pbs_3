using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class Description : IValueObject
{
    public Description()
    {
    } 
    public Description(string bookDescription)
    {
        if (string.IsNullOrEmpty(bookDescription))
            throw new BusinessRulesException("Description cannot be null or empty");
        if (bookDescription.Length > 1000)
            throw new ArgumentException("Description cannot have more than 1000 characters");
        BookDescription = bookDescription.Trim();
    }

    public string BookDescription { get; }


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

        var that = (Description)obj;

        return BookDescription.ToUpper().Equals(that.BookDescription.ToUpper());
    }

    public override string ToString()
    {
        return $"{BookDescription}";
    }

    public override int GetHashCode()
    {
        return BookDescription.GetHashCode();
    }
}