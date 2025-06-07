using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class Rental: Entity<RentalID>, IAggregateRoot
{
    private RentalID IdRental;
    private RentalPeriod PeriodOfTheRental;
    private Status RentalStatus;
    private String BookID;

    public Rental(string rentalId, string startDateTime, string endDateTime, int statusId)
    {
        IdRental = new RentalID(rentalId);

        if (!DateTime.TryParse(startDateTime, out var start))
            throw new ArgumentException("Invalid start date/time format.");
        if (!DateTime.TryParse(endDateTime, out var end))
            throw new ArgumentException("Invalid end date/time format.");

        PeriodOfTheRental = new RentalPeriod(start, end);

        var status = BookingStatusHelper.GetStatusById(statusId);
        if (status == null)
            throw new ArgumentException("Invalid status id.");

        RentalStatus = status.Value;
    }

    public void CancelBooking()
    {
        RentalStatus = Status.Cancelled;
    }

    public void MarkAsCompleted()
    {
        RentalStatus=  Status.Completed;
    }

    public void MarkAsActive()
    {
        RentalStatus= Status.Active;
    }

    public void MarkAsPending()
    {
        RentalStatus= Status.Pending;
    }

    public Status GetCurrentStatus()
    {
        return RentalStatus;
    }
}