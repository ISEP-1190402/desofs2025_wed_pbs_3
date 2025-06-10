using LibraryOnlineRentalSystem.Domain.Common;
using Newtonsoft.Json;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class Rental : Entity<RentalID>, IAggregateRoot
{
    public RentalID Id { get; private set; }
    public RentalStartDate StartDate { get; private set; }
    public RentalEndDate EndDate { get; private set; }
    public RentedBookID RentedBookIdentifier { get; private set; }
    public RentalStatus StatusOfRental { get; private set; }
    public UserEmail EmailUser { get; private set; }

    public Rental()
    {
    }

    [JsonConstructor]
    public Rental(string rentalId, string startDateTime, string endDateTime, string bookId, string userEmail)
    {
        Id = new RentalID(rentalId);

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
            Id.Value,
            StartDate.StartDateTime.ToString("o"),
            EndDate.EndDateTime.ToString("o"),
            RentedBookIdentifier.GetRentedBookId(),
            StatusOfRental.RentStatus.ToString(),
            EmailUser.ToString()
        );
    }
}