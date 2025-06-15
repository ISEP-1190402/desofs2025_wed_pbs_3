using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.User;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class Rental : Entity<RentalID>, IAggregateRoot
{
    public RentalID Id { get; private set; }
    public RentalStartDate StartDate { get; private set; }
    public RentalEndDate EndDate { get; private set; }
    public RentedBookID RentedBookIdentifier { get; private set; }
    public RentalStatus StatusOfRental { get; private set; }
    public UserEmail EmailUser { get; private set; }
    private static ILogger<RentalService> _logger;

    public static void ConfigureLogger(ILogger<RentalService> logger)
    {
        _logger = logger;
    }

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

        _logger?.LogInformation("Rental created with ID: {RentalID}, BookID: {BookID}, User: {UserEmail}", Id.Value, bookId, userEmail);

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
        _logger?.LogInformation("Attempting to create rental for BookID: {BookID} and User: {UserEmail}", bookId, userEmail);

        // Validate book exists and has available copies
        var book = bookRepository.GetByIdAsync(new BookID(bookId)).Result;
        if (book == null)
        {
            _logger?.LogWarning("Rental creation failed: Book with ID {BookID} not found", bookId);

            throw new BusinessRulesException("Book not found");
        }

        if (book.AmountOfCopies.GetBookAmountOfCopies() <= 0)
        {
            _logger?.LogWarning("Rental creation failed: No available copies for BookID {BookID}", bookId);

            throw new BusinessRulesException("No available copies of this book");
        }

        // Validate user exists
        var user = userRepository.GetByEmailAsync(userEmail).Result;
        if (user == null)
        {
            _logger?.LogWarning("Rental creation failed: User with email {UserEmail} not found", userEmail);

            throw new BusinessRulesException("User not found");
        }

        _logger?.LogInformation("Rental validation passed for BookID: {BookID} and User: {UserEmail}", bookId, userEmail);


        var rental = new Rental(rentalId, startDateTime, endDateTime, bookId, userEmail);
        return rental;
    }

    public void CancelBooking()
    {
        _logger?.LogInformation("Cancelling rental ID: {RentalID}", Id.Value);

        if (StatusOfRental.RentStatus == Status.Completed)
        {
            _logger?.LogWarning("Cannot cancel a completed rental (ID: {RentalID})", Id.Value);

            throw new BusinessRulesException("Cannot cancel a completed rental");
        }

        if (StatusOfRental.RentStatus == Status.Cancelled)
        {
            _logger?.LogWarning("Rental (ID: {RentalID}) is already cancelled", Id.Value);

            throw new BusinessRulesException("Rental is already cancelled");
        }

        StatusOfRental = new RentalStatus(Status.Cancelled);
        _logger?.LogInformation("Rental ID {RentalID} marked as Cancelled", Id.Value);

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
        _logger?.LogInformation("Rental ID {RentalID} marked as Active", Id.Value);

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
        _logger?.LogInformation("Rental ID {RentalID} marked as Pending", Id.Value);

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
        _logger?.LogInformation("Rental ID {RentalID} marked as Completed", Id.Value);

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
        var overdue = DateTime.UtcNow > EndDate.EndDateTime && !IsCompleted() && !IsCancelled();
        if (overdue)
            _logger?.LogInformation("Rental ID {RentalID} is overdue", Id.Value);
        return overdue;
    }
}