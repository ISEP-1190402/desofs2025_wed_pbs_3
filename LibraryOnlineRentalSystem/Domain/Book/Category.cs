namespace LibraryOnlineRentalSystem.Domain.Book;

public class Category
{
    public string BookCategoryName { get; private set; }


    public Category(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Category cannot be null or empty");
        }

        this.BookCategoryName = name.Trim();
    }


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
}