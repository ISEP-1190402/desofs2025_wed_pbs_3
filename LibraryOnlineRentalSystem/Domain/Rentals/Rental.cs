using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class Rental: Entity<RentalID>, IAggregateRoot
{
    private RentalID _rentalId;
    private RentalPeriod _rentalPeriod;
    private Status _status;

    public Rental(string rentalId, string startDateTime, string endDateTime, int statusId)
    {
        _rentalId = new RentalID(rentalId);

        if (!DateTime.TryParse(startDateTime, out var start))
            throw new ArgumentException("Invalid start date/time format.");
        if (!DateTime.TryParse(endDateTime, out var end))
            throw new ArgumentException("Invalid end date/time format.");

        _rentalPeriod = new RentalPeriod(start, end);

        var status = BookingStatusHelper.GetStatusById(statusId);
        if (status == null)
            throw new ArgumentException("Invalid status id.");

        _status = status.Value;
    }

    public void CancelBooking()
    {
        _status = Status.Cancelled;
    }

    public void MarkAsCompleted()
    {
        _status=  Status.Completed;
    }

    public void MarkAsActive()
    {
        _status= Status.Active;
    }

    public void MarkAsPending()
    {
        _status= Status.Pending;
    }

    public Status GetCurrentStatus()
    {
        return _status;
    }
}