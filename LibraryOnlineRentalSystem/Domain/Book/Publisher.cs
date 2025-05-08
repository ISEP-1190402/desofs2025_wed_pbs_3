namespace LibraryOnlineRentalSystem.Domain.Book;

public class Publisher
{
    public string PublisherName { get; private set; }


    public Publisher(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Publisher cannot be null or empty");
        }

        this.PublisherName = name.Trim();
    }


    public string GetBookPublisher()
    {
        return PublisherName;
    }


    public Publisher ValueOf(string name)
    {
        return new Publisher(name);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;

        if (obj == null || obj.GetType() != GetType()) return false;

        var that = (Description)obj;

        return PublisherName.ToUpper().Equals(that.BookDescription.ToUpper());
    }

    public override string ToString()
    {
        return $"{PublisherName}";
    }
}