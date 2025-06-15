using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentalStartDate : IValueObject
{
    public DateTime StartDateTime { get; private set; }

    protected RentalStartDate() { }

    public RentalStartDate(DateTime startDateTime)
    {
        if (startDateTime < DateTime.UtcNow.AddMinutes(-5)) // Allow 5 minutes of clock skew
        {
            throw new BusinessRulesException("Start date cannot be in the past");
        }

        if (startDateTime > DateTime.UtcNow.AddYears(1)) // Max 1 year in the future
        {
            throw new BusinessRulesException("Start date cannot be more than 1 year in the future");
        }

        StartDateTime = startDateTime;
    }


    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;
        var that = (RentalStartDate)obj;
        return StartDateTime == that.StartDateTime;
    }

    public override int GetHashCode() => StartDateTime.GetHashCode();
    public override string ToString() => StartDateTime.ToString("o");
}