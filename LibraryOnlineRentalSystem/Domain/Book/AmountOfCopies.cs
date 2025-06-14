using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class AmountOfCopies : IValueObject
{
    private const int MinCopies = 0;
    private const int MaxCopies = 1500;

    public AmountOfCopies()
    {
        BookAmountOfCopies = MinCopies;
    }
    
    public AmountOfCopies(int stock)
    {
        if (stock < MinCopies)
            throw new BusinessRulesException($"Amount of copies cannot be less than {MinCopies}");
            
        if (stock > MaxCopies)
            throw new BusinessRulesException($"Amount of copies cannot exceed {MaxCopies}");

        BookAmountOfCopies = stock;
    }

    public int BookAmountOfCopies { get; }

    public int GetBookAmountOfCopies()
    {
        return BookAmountOfCopies;
    }

    public AmountOfCopies ValueOf(int stock)
    {
        return new AmountOfCopies(stock);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not AmountOfCopies other) return false;
        return BookAmountOfCopies == other.BookAmountOfCopies;
    }

    public override string ToString()
    {
        return BookAmountOfCopies.ToString();
    }

    public override int GetHashCode()
    {
        return BookAmountOfCopies.GetHashCode();
    }
}