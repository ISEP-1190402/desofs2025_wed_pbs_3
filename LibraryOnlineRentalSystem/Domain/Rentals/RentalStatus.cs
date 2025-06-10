using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentalStatus : IValueObject
{
    public RentalStatus()
    {
    }

    public RentalStatus(Status status)
    {
        RentStatus = status;
    }

    public Status RentStatus { get; }


    public string GetStatus()
    {
        return RentStatus.ToString();
    }


    public RentalStatus ValueOf(Status status)
    {
        return new RentalStatus(status);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;

        if (obj == null || obj.GetType() != GetType()) return false;

        var that = (RentalStatus)obj;

        return BookingStatusHelper.GetIdByStatus(RentStatus) == BookingStatusHelper.GetIdByStatus(that.RentStatus);
    }

    public override string ToString()
    {
        return $"{RentStatus}";
    }

    public override int GetHashCode()
    {
        return RentStatus.GetHashCode();
    }
}