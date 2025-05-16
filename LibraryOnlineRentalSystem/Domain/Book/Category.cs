using LibraryOnlineRentalSystem.Domain.Shared;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class Category : IValueObject
{
    public Category(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new BusinessRulesException("Category cannot be null or empty");

        BookCategoryName = name.Trim();
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

        if (obj == null || obj.GetType() != GetType()) return false;

        var that = (Category)obj;

        return BookCategoryName.ToUpper().Equals(that.BookCategoryName.ToUpper());
    }

    public override string ToString()
    {
        return $"{BookCategoryName}";
    }

    public override int GetHashCode()
    {
        return BookCategoryName.GetHashCode();
    }
}