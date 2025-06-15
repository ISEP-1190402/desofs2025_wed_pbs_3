using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.User;
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

    // Required by EF Core
    protected Rental() { }

    [JsonConstructor]
    public Rental(string rentalId, string startDateTime, string endDateTime, string bookId, string userEmail)
        : this(rentalId, 
              DateTime.Parse(startDateTime), 
              DateTime.Parse(endDateTime), 
              bookId, 
              userEmail)
    {
    }

    public Rental(string rentalId, DateTime startDateTime, DateTime endDateTime, string bookId, string userEmail)
    {
        Id = new RentalID(rentalId);
        StartDate = new RentalStartDate(startDateTime);
        // Pass the start date to validate against the end date
        EndDate = new RentalEndDate(endDateTime, startDateTime);
        RentedBookIdentifier = new RentedBookID(bookId);
        EmailUser = new UserEmail(userEmail);
        StatusOfRental = new RentalStatus(Status.Pending); // Start as Pending, will be confirmed after validation
    }

    public static Rental Create(
        string rentalId,
        DateTime startDateTime,
        DateTime endDateTime,
        string bookId,
        string userEmail,
        IBookRepository bookRepository,
        IUserRepository userRepository)
    {
        // Validate book exists and has available copies
        var book = bookRepository.GetByIdAsync(new BookID(bookId)).Result;
        if (book == null)
        {
            throw new BusinessRulesException("Book not found");
        }

        if (book.AmountOfCopies.GetBookAmountOfCopies() <= 0)
        {
            throw new BusinessRulesException("No available copies of this book");
        }

        // Validate user exists
        var user = userRepository.GetByEmailAsync(userEmail).Result;
        if (user == null)
        {
            throw new BusinessRulesException("User not found");
        }

        var rental = new Rental(rentalId, startDateTime, endDateTime, bookId, userEmail);
        return rental;
    }

    public void CancelBooking()
    {
        if (StatusOfRental.RentStatus == Status.Completed)
        {
            throw new BusinessRulesException("Cannot cancel a completed rental");
        }

        if (StatusOfRental.RentStatus == Status.Cancelled)
        {
            throw new BusinessRulesException("Rental is already cancelled");
        }

        StatusOfRental = new RentalStatus(Status.Cancelled);
    }

    public void MarkAsActive()
    {
        if (StatusOfRental.RentStatus == Status.Cancelled)
        {
            throw new BusinessRulesException("Cannot activate a cancelled rental");
        }

        if (StatusOfRental.RentStatus == Status.Completed)
        {
            throw new BusinessRulesException("Cannot activate a completed rental");
        }

        StatusOfRental = new RentalStatus(Status.Active);
    }
    
    public void MarkAsPending()
    {
        if (StatusOfRental.RentStatus == Status.Cancelled)
        {
            throw new BusinessRulesException("Cannot set a cancelled rental to pending");
        }

        if (StatusOfRental.RentStatus == Status.Completed)
        {
            throw new BusinessRulesException("Cannot set a completed rental to pending");
        }

        StatusOfRental = new RentalStatus(Status.Pending);
    }
    
    /// <summary>
    /// Marks the rental as Completed
    /// </summary>
    /// <exception cref="BusinessRulesException">Thrown if the rental is cancelled</exception>
    public void MarkAsCompleted()
    {
        if (StatusOfRental.RentStatus == Status.Cancelled)
        {
            throw new BusinessRulesException("Cannot complete a cancelled rental");
        }
        
        if (StatusOfRental.RentStatus == Status.Completed)
        {
            throw new BusinessRulesException("Rental is already completed");
        }
        
        StatusOfRental = new RentalStatus(Status.Completed);
    }

    public bool IsActive() => StatusOfRental.RentStatus == Status.Active;
    public bool IsCompleted() => StatusOfRental.RentStatus == Status.Completed;
    public bool IsCancelled() => StatusOfRental.RentStatus == Status.Cancelled;
    public bool IsPending() => StatusOfRental.RentStatus == Status.Pending;

    public RentalStatus GetCurrentStatus() => StatusOfRental;

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

    // Business method to check if rental is overdue
    public bool IsOverdue()
    {
        return DateTime.UtcNow > EndDate.EndDateTime && !IsCompleted() && !IsCancelled();
    }
}