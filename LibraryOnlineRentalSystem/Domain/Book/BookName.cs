using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class BookName : IValueObject
{
    private const int MinLength = 2;
    private const int MaxLength = 200;
    private static readonly Regex ValidBookNameRegex = new Regex("^[\\p{L}\\d\\s.,:;!?()&'\"-]+$", RegexOptions.Compiled);

    public BookName()
    {
        NameBook = string.Empty;
    }
    
    public BookName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRulesException("Book name cannot be null or empty");

        name = name.Trim();

        if (name.Length < MinLength)
            throw new BusinessRulesException($"Book name must be at least {MinLength} characters long");

        if (name.Length > MaxLength)
            throw new BusinessRulesException($"Book name cannot exceed {MaxLength} characters");

        if (!ValidBookNameRegex.IsMatch(name))
            throw new BusinessRulesException(
                "Book name contains invalid characters. Only letters, numbers, spaces, and basic punctuation are allowed.");

        NameBook = name;
    }

    public string NameBook { get; }

    public string GetValue()
    {
        return NameBook;
    }

    public BookName ValueOf(string name)
    {
        return new BookName(name);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not BookName other) return false;
        return string.Equals(NameBook, other.NameBook, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return NameBook;
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(NameBook);
    }
}
