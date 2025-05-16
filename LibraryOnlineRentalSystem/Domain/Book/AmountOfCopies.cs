namespace LibraryOnlineRentalSystem.Domain.Book;

public class AmountOfCopies : IValueObject
{
    public AmountOfCopies(int numberOfCopies)
    {
        if (numberOfCopies < 0) throw new BusinessRulesException("Stock cannot be negative");

        BookAmountOfCopies = numberOfCopies;
    }

    public int BookAmountOfCopies { get; }


    public int GetBookAmountOfCopies()
    {
        return BookAmountOfCopies;
    }


    public AmountOfCopies ValueOf(int numberOfCopies)
    {
        return new AmountOfCopies(numberOfCopies);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;

        if (obj == null || obj.GetType() != GetType()) return false;

        var that = (AmountOfCopies)obj;

        return BookAmountOfCopies == that.BookAmountOfCopies;
    }

    public override string ToString()
    {
        return $"{BookAmountOfCopies.ToString()}";
    }

    public override int GetHashCode()
    {
        return BookAmountOfCopies.GetHashCode();
    }
}