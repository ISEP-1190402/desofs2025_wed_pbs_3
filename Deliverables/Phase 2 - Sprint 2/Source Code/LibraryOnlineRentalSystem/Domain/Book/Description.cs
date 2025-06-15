using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class Description : IValueObject
{
    private static readonly Regex ValidDescriptionRegex = new Regex(@"^[\p{L}\p{N}\p{P}\p{S}\s\r\n-]+", RegexOptions.Compiled);
    private static readonly string[] XssPatterns = 
    {
        "<script", "javascript:", "onload=", "onerror=", "onclick=",
        "onmouseover=", "document.cookie", "eval(", "alert(",
        "fromcharcode(", "document.domain", "window.location",
        "document.write", "innerhtml", "settimeout(", "setinterval("
    };

    public Description()
    {
        BookDescription = string.Empty;
    }
    
    public Description(string bookDescription)
    {
        if (string.IsNullOrWhiteSpace(bookDescription))
            throw new BusinessRulesException("Description cannot be null or empty");

        bookDescription = bookDescription.Trim();

        if (bookDescription.Length < 5)
            throw new BusinessRulesException("Description must be at least 5 characters long");

        if (bookDescription.Length > 2000)
            throw new BusinessRulesException("Description cannot exceed 2000 characters");

        if (ContainsXssPatterns(bookDescription))
            throw new BusinessRulesException("Description contains potentially harmful content");

        if (!ValidDescriptionRegex.IsMatch(bookDescription))
            throw new BusinessRulesException("Description contains invalid characters");

        // Basic HTML tag stripping as an additional safety measure
        BookDescription = StripHtmlTags(bookDescription);
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
        if (obj is not Description other) return false;
        return string.Equals(BookDescription, other.BookDescription, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return BookDescription;
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(BookDescription);
    }


    private static bool ContainsXssPatterns(string input)
    {
        var lowerInput = input.ToLowerInvariant();
        return XssPatterns.Any(pattern => lowerInput.Contains(pattern));
    }

    private static string StripHtmlTags(string input)
    {
        // Basic HTML tag stripping - for more comprehensive protection, consider using a library like HtmlSanitizer
        return Regex.Replace(input, "<[^>]*(>|$)", string.Empty);
    }
}