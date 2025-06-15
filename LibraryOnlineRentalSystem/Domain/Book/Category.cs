using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class Category : IValueObject
{
    private const int MinLength = 2;
    private const int MaxLength = 50;
    private static readonly Regex ValidCategoryRegex = new(@"^[\p{L}\s-]+$", RegexOptions.Compiled);

    public Category()
    {
        BookCategoryName = string.Empty;
    }
    
    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRulesException("Category cannot be null or empty");

        name = name.Trim();

        if (name.Length < MinLength)
            throw new BusinessRulesException($"Category name must be at least {MinLength} characters long");

        if (name.Length > MaxLength)
            throw new BusinessRulesException($"Category name cannot exceed {MaxLength} characters");

        if (!ValidCategoryRegex.IsMatch(name))
            throw new BusinessRulesException(
                "Category name contains invalid characters. Only letters, spaces, and hyphens are allowed.");

        BookCategoryName = name;
    }

    public string BookCategoryName { get; }

    public string GetBookCategoryName()
    {
        return BookCategoryName;
    }

    public Category ValueOf(string name)
    {
        return new Category(name);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not Category other) return false;
        return string.Equals(BookCategoryName, other.BookCategoryName, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return BookCategoryName;
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(BookCategoryName);
    }
}