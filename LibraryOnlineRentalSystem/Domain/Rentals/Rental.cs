using LibraryOnlineRentalSystem.Domain.Common;
using Newtonsoft.Json;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class Rental : Entity<RentalID>, IAggregateRoot
{
    public RentalID IdRental;
    public RentalStartDate StartDate;
    public RentalEndDate EndDate;
    public RentedBookID RentedBookIdentifier;
    public RentalStatus StatusOfRental;
    public UserEmail EmailUser;

    public Rental()
    {
    }

    [JsonConstructor]
    public Rental(string rentalId, string startDateTime, string endDateTime, string bookId, string userEmail)
    {
        IdRental = new RentalID(rentalId);

        if (!DateTime.TryParse(startDateTime, out var start))
            throw new ArgumentException("Invalid start date/time format.");
        if (!DateTime.TryParse(endDateTime, out var end))
            throw new ArgumentException("Invalid end date/time format.");

        StartDate = new RentalStartDate(start);
        EndDate = new RentalEndDate(end);
        RentedBookIdentifier = new RentedBookID(bookId);
        StatusOfRental = new RentalStatus(Status.Active);
        EmailUser = new UserEmail(userEmail);
    }

    public void CancelBooking()
    {
        StatusOfRental = new RentalStatus(Status.Cancelled);
    }

    public void MarkAsCompleted()
    {
        StatusOfRental = new RentalStatus(Status.Completed);
    }

    public void MarkAsActive()
    {
        StatusOfRental = new RentalStatus(Status.Active);
    }

    public void MarkAsPending()
    {
        StatusOfRental = new RentalStatus(Status.Pending);
    }

    public RentalStatus GetCurrentStatus()
    {
        return StatusOfRental;
    }

    public RentalDTO toDTO()
    {
        return new RentalDTO(
            IdRental.Value,
            StartDate.StartDateTime.ToString("o"),
            EndDate.EndDateTime.ToString("o"),
            RentedBookIdentifier.GetRentedBookId(),
            StatusOfRental.RentStatus.ToString(),
            EmailUser.ToString()
        );
    }
}