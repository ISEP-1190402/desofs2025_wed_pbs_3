using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class Author : IValueObject
{
    private static readonly Regex ValidAuthorRegex = new Regex(@"^[\p{L}\s.'-]+$", RegexOptions.Compiled);

    public Author()
    {
        BookAuthor = string.Empty;
    }  
    
    public Author(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new BusinessRulesException("Author name cannot be null or empty");

        author = author.Trim();

        if (author.Length < 2)
            throw new BusinessRulesException("Author name must be at least 2 characters long");

        if (author.Length > 100)
            throw new BusinessRulesException("Author name cannot exceed 100 characters");

        if (!ValidAuthorRegex.IsMatch(author))
            throw new BusinessRulesException("Author name contains invalid characters. Only letters, spaces, hyphens (-), apostrophes ('), and periods (.) are allowed.");

        BookAuthor = author;
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
        if (obj is not Author other) return false;
        return string.Equals(BookAuthor, other.BookAuthor, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return BookAuthor;
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(BookAuthor);
    }
}