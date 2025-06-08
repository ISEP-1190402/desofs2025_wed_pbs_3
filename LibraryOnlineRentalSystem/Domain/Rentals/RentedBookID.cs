using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentedBookID
{
    public RentedBookID()
    {
    }
    public RentedBookID(string rentalBookId)
    {
        BookId = rentalBookId ?? throw new BusinessRulesException("Book cannot be null");
    }

    private string BookId { get; }

    public string GetRentedBookId()
    {
        return BookId;
    }

    public RentedBookID ValueOf(string rentalBookId)
    {
        return new RentedBookID(rentalBookId);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;

        if (obj == null || obj.GetType() != GetType()) return false;

        var that = (RentedBookID)obj;

        return BookId.ToUpper().Equals(that.BookId.ToUpper());
    }

    public override string ToString()
    {
        return $"{BookId}";
    }

    public override int GetHashCode()
    {
        return BookId.GetHashCode();
    }
}