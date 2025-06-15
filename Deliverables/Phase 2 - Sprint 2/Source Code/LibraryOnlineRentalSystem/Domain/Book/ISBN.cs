using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class ISBN : IValueObject
{
    private static readonly Regex Isbn10Regex = new Regex(@"^\d{9}[\dXx]$", RegexOptions.Compiled);
    private static readonly Regex Isbn13Regex = new Regex(@"^\d{13}$", RegexOptions.Compiled);
    private static readonly Regex IsbnFormatRegex = new Regex(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$", RegexOptions.Compiled);

    public ISBN()
    {
        BookISBN = string.Empty;
    }
    
    private static Action<string> _validateIsbnUniqueness;
    
    // This method should be called during application startup
    public static void Configure(Action<string> validateIsbnUniqueness)
    {
        _validateIsbnUniqueness = validateIsbnUniqueness ?? throw new ArgumentNullException(nameof(validateIsbnUniqueness));
    }

    public ISBN(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new BusinessRulesException("ISBN cannot be null or empty");

        isbn = isbn.Trim();
        string cleanIsbn = isbn.Replace("-", "").Replace(" ", "").ToUpper();

        if (!IsValidFormat(cleanIsbn))
            throw new BusinessRulesException("Invalid ISBN format. Please use a valid ISBN-10 or ISBN-13 format.");

        // Validate ISBN uniqueness using the injected validation action
        try
        {
            _validateIsbnUniqueness?.Invoke(cleanIsbn);
        }
        catch (BusinessRulesException ex)
        {
            throw new BusinessRulesException(ex.Message);
        }

        BookISBN = cleanIsbn; 
    }
    



    public string BookISBN { get; }

    public string GetISBN()
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
        if (obj is not ISBN other) return false;
        return string.Equals(BookISBN, other.BookISBN, StringComparison.Ordinal);
    }

    public override string ToString()
    {
        return BookISBN;
    }

    public override int GetHashCode()
    {
        return StringComparer.Ordinal.GetHashCode(BookISBN);
    }

    private static bool IsValidFormat(string isbn)
    {
      
        string cleanIsbn = Regex.Replace(isbn, "[^0-9X]", "", RegexOptions.IgnoreCase).ToUpper();

      
        return (cleanIsbn.Length == 10 && Isbn10Regex.IsMatch(cleanIsbn)) ||
               (cleanIsbn.Length == 13 && Isbn13Regex.IsMatch(cleanIsbn));
    }
}