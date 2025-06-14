using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class Publisher : IValueObject
{
    private static readonly Regex ValidPublisherRegex = new Regex(@"^[\p{L}0-9 .,'&()\-]+", RegexOptions.Compiled);

    public Publisher()
    {
        PublisherName = string.Empty;
    }
    
    public Publisher(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRulesException("Publisher name cannot be null or empty");

        name = name.Trim();

        if (name.Length < 2)
            throw new BusinessRulesException("Publisher name must be at least 2 characters long");

        if (name.Length > 100)
            throw new BusinessRulesException("Publisher name cannot exceed 100 characters");

        if (!ValidPublisherRegex.IsMatch(name))
            throw new BusinessRulesException("Publisher name contains invalid characters. Only letters, numbers, spaces, and basic punctuation are allowed.");

  
        PublisherName = name;
    }


    public string PublisherName { get; }

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
        if (obj is not Publisher other) return false;
        return string.Equals(PublisherName, other.PublisherName, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return PublisherName;
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(PublisherName);
    }


  

    
}