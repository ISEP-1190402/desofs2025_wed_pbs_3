using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.User;
using Microsoft.Extensions.Logging;
using BusinessRulesException = LibraryOnlineRentalSystem.Domain.Common.BusinessRulesException;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWorkUnity _workUnity;
    private readonly ILogger<RentalService> _logger;

    public RentalService(
        IRentalRepository rentalRepository,
        IBookRepository bookRepository,
        IUserRepository userRepository,
        IWorkUnity workUnity,
        ILogger<RentalService> logger)
    {
        _rentalRepository = rentalRepository;
        _bookRepository = bookRepository;
        _userRepository = userRepository;
        _workUnity = workUnity;
        _logger = logger;
    }

    // USER - Rent a Book
    public async Task<RentalDTO> CreateARentalAsync(CreatedRentalDTO rentalDto)
    {
        try
        {
            // Validate input
            if (rentalDto == null)
                throw new BusinessRulesException("Rental data cannot be empty");

            // Generate a new ID
            var id = (await _rentalRepository.GetAllAsync()).Count + 1 + "";

            // Parse dates
            if (!DateTime.TryParse(rentalDto.StartDate, out var startDate))
                throw new BusinessRulesException("Invalid start date format");

            if (!DateTime.TryParse(rentalDto.EndDate, out var endDate))
                throw new BusinessRulesException("Invalid end date format");

            // Check if the book is available for rental
            var isBookAvailable = await IsBookAvailableForRental(rentalDto.ReservedBookId);
            if (!isBookAvailable)
            {
                throw new BusinessRulesException("The selected book is not available for the requested period");
            }

            // Create the rental with validation
            var rental = Rental.Create(
                id,
                startDate,
                endDate,
                rentalDto.ReservedBookId,
                rentalDto.UserEmail,
                _bookRepository,
                _userRepository);

            // Get the book first to check current stock
            var book = await _bookRepository.GetByIdAsync(new BookID(rentalDto.ReservedBookId));
            if (book == null)
            {
                throw new BusinessRulesException("Book not found");
            }

            // Decrement book stock using the repository method
            var updatedBook = _bookRepository.UpdateBookStock(rentalDto.ReservedBookId, book.AmountOfCopies.GetBookAmountOfCopies() - 1);
            if (updatedBook == null)
            {
                throw new BusinessRulesException("Failed to update book stock");
            }

            // Set status to Active
            rental.MarkAsActive();
        
            // Save changes
            await _rentalRepository.AddAsync(rental);
            await _workUnity.CommitAsync();

            _logger.LogInformation($"Rental {rental.Id} created successfully for user {rentalDto.UserEmail} with status: {rental.GetCurrentStatus().RentStatus}");

            return rental.toDTO();
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning($"Failed to create rental: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a rental");
            throw new BusinessRulesException("An error occurred while processing your request");
        }
    }

    private async Task<bool> IsBookAvailableForRental(string bookId)
    {
        // Get the book and check availability
        var book = await _bookRepository.GetByIdAsync(new BookID(bookId));
        if (book == null)
        {
            throw new BusinessRulesException("Book not found");
        }

        // Check if there are available copies
        if (book.AmountOfCopies.GetBookAmountOfCopies() <= 0)
        {
            throw new BusinessRulesException("No available copies of this book");
        }

        return true;
    }

    // USER - Cancel a Rental
    public async Task<RentalDTO> CancelARental(string rentalID)
    {
        return await UpdateRentalStatusAsync(rentalID, "cancelled");
    }
    
    public async Task<RentalDTO> UpdateRentalStatusAsync(string rentalId, string newStatus)
    {
        if (string.IsNullOrWhiteSpace(rentalId))
        {
            _logger.LogWarning("Attempted to update status with null or empty rental ID");
            throw new ArgumentException("Rental ID is required", nameof(rentalId));
        }

        if (string.IsNullOrWhiteSpace(newStatus))
        {
            _logger.LogWarning("Attempted to update rental {RentalId} with empty status", rentalId);
            throw new ArgumentException("Status is required", nameof(newStatus));
        }

        _logger.LogDebug("Looking up rental with ID: {RentalId}", rentalId);
        var rental = await _rentalRepository.GetByIdAsync(new RentalID(rentalId));
        if (rental == null)
        {
            _logger.LogWarning("Rental with ID {RentalId} not found", rentalId);
            throw new BusinessRulesException($"Rental with ID {rentalId} not found");
        }

        _logger.LogDebug("Current status of rental {RentalId}: {CurrentStatus}", rentalId, rental.GetCurrentStatus().RentStatus);
        _logger.LogDebug("Requested new status: {NewStatus}", newStatus);

        // Update status based on the provided value
        try
        {
            switch (newStatus.ToLower())
            {
                case "pending":
                    rental.MarkAsPending();
                    break;
                case "active":
                    rental.MarkAsActive();
                    break;
                case "completed":
                    rental.MarkAsCompleted();
                    break;
                case "cancelled":
                    rental.CancelBooking();
                    break;
                default:
                    var errorMessage = $"Invalid status: {newStatus}. Valid statuses are: pending, active, completed, cancelled";
                    _logger.LogWarning(errorMessage);
                    throw new BusinessRulesException(errorMessage);
            }

            _logger.LogDebug("Status updated in memory. Saving changes...");
            _rentalRepository.Update(rental);
            await _workUnity.CommitAsync();

            _logger.LogInformation("Successfully updated status of rental {RentalId} to {Status}", rentalId, newStatus);
            return rental.toDTO();
        }
        catch (BusinessRulesException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while updating rental {RentalId}: {Message}", rentalId, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating status of rental {RentalId}", rentalId);
            throw new BusinessRulesException($"An error occurred while updating the rental status: {ex.Message}");
        }
    }

    public async Task<RentalDTO> GetRentalByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Attempted to get rental with null or empty ID");
            throw new ArgumentNullException(nameof(id), "Rental ID cannot be null or empty");
        }

        try
        {
            _logger.LogDebug("Attempting to retrieve rental with ID: {RentalId}", id);
            var rentalId = new RentalID(id);
            var rental = await _rentalRepository.GetByIdAsync(rentalId);
            
            if (rental == null)
            {
                _logger.LogWarning("Rental with ID {RentalId} not found", id);
                return null;
            }

            _logger.LogDebug("Successfully retrieved rental with ID: {RentalId}", id);
            return rental.toDTO();
        }
        catch (Exception ex) when (ex is not ArgumentNullException)
        {
            _logger.LogError(ex, "Error retrieving rental with ID {RentalId}", id);
            throw new BusinessRulesException($"An error occurred while retrieving rental with ID {id}", ex.Message);
        }
    }

    // USER - GET ALL Rentals
    public async Task<List<RentalDTO>> GetAllRentalsOfUserAsync(string userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            _logger.LogWarning("Attempted to get rentals with null or empty email");
            throw new ArgumentException("Email is required", nameof(userEmail));
        }

        _logger.LogDebug("Validating user with email: {UserEmail}", userEmail);
        var user = await _userRepository.GetByEmailAsync(userEmail);
        if (user == null)
        {
            _logger.LogWarning("User with email {UserEmail} not found", userEmail);
            throw new BusinessRulesException($"User with email {userEmail} not found");
        }

        _logger.LogDebug("Fetching rentals for user: {UserEmail}", userEmail);
        var rentals = await _rentalRepository.GetAllAsyncOfUser(userEmail);
        
        if (rentals == null || !rentals.Any())
        {
            _logger.LogInformation("No rentals found for user: {UserEmail}", userEmail);
            return new List<RentalDTO>();
        }

        _logger.LogInformation("Found {Count} rentals for user: {UserEmail}", rentals.Count, userEmail);
        return rentals.ConvertAll(rental => rental.toDTO());
    }

    // USER - GET Active Rentals
    public async Task<List<RentalDTO>> GetAllActiveRentalsOfUserAsync(string userEmail)
    {
        await ValidateUserEmailAsync(userEmail);
        _logger.LogDebug("Fetching active rentals for user: {UserEmail}", userEmail);
        var rentals = await _rentalRepository.GetAllActiveRentalsOfUserAssync(userEmail);
        return HandleRentalsResult(rentals, userEmail, "active");
    }

    // USER - GET Cancelled Rentals
    public async Task<List<RentalDTO>> GetAllCancelledRentalsOfUserAsync(string userEmail)
    {
        await ValidateUserEmailAsync(userEmail);
        _logger.LogDebug("Fetching cancelled rentals for user: {UserEmail}", userEmail);
        var rentals = await _rentalRepository.GetAllCancelledRentalsOfUserAssync(userEmail);
        return HandleRentalsResult(rentals, userEmail, "cancelled");
    }

    // USER - GET Pending Rentals
    public async Task<List<RentalDTO>> GetAllPendingRentalsOfUserAsync(string userEmail)
    {
        await ValidateUserEmailAsync(userEmail);
        _logger.LogDebug("Fetching pending rentals for user: {UserEmail}", userEmail);
        var rentals = await _rentalRepository.GetAllPendingRentalsOfUserAssync(userEmail);
        return HandleRentalsResult(rentals, userEmail, "pending");
    }

    // Library Manager - GET Completed Rentals
    public async Task<List<RentalDTO>> GetAllCompletedRentalsOfUserAsync(string userEmail)
    {
        await ValidateUserEmailAsync(userEmail);
        _logger.LogDebug("Fetching completed rentals for user: {UserEmail}", userEmail);
        var rentals = await _rentalRepository.GetAllCompleteRentalsOfUserAssync(userEmail);
        return HandleRentalsResult(rentals, userEmail, "completed");
    }
    
    private async Task ValidateUserEmailAsync(string userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            _logger.LogWarning("Attempted to access with null or empty email");
            throw new ArgumentException("Email is required", nameof(userEmail));
        }

        _logger.LogDebug("Validating user with email: {UserEmail}", userEmail);
        var user = await _userRepository.GetByEmailAsync(userEmail);
        if (user == null)
        {
            _logger.LogWarning("User with email {UserEmail} not found", userEmail);
            throw new BusinessRulesException($"User with email {userEmail} not found");
        }
    }
    
    private List<RentalDTO> HandleRentalsResult(List<Rental> rentals, string userEmail, string rentalType)
    {
        if (rentals == null || !rentals.Any())
        {
            _logger.LogInformation("No {RentalType} rentals found for user: {UserEmail}", rentalType, userEmail);
            return new List<RentalDTO>();
        }

        _logger.LogInformation("Found {Count} {RentalType} rentals for user: {UserEmail}", 
            rentals.Count, rentalType, userEmail);
        return rentals.ConvertAll(rental => rental.toDTO());
    }

    // Library Manager - GET ALL Rentals
    public async Task<List<RentalDTO>> GetAllRentalsAsync()
    {
        var list = await _rentalRepository.GetAllAsync();
        var listDto = list.ConvertAll(rental =>
            rental.toDTO());
        return listDto;
    }

    // Library Manager - GET Active Rentals
    public async Task<List<RentalDTO>> GetAllActiveRentalsAsync()
    {
        var list = await _rentalRepository.GetAllActiveRentalsAssync();
        var listDto = list.ConvertAll(rental =>
            rental.toDTO());
        return listDto;
    }

    // Library Manager - GET Cancelled Rentals
    public async Task<List<RentalDTO>> GetAllCancelledRentalsAsync()
    {
        var list = await _rentalRepository.GetAllCancelledRentalsAssync();
        var listDto = list.ConvertAll(rental =>
            rental.toDTO());
        return listDto;
    }

    // Library Manager - GET Pending Rentals
    public async Task<List<RentalDTO>> GetAllPendingRentalsAsync()
    {
        var list = await _rentalRepository.GetAllPendingRentalsAssync();
        var listDto = list.ConvertAll(rental =>
            rental.toDTO());
        return listDto;
    }

    // Library Manager - GET Active Rentals
    public async Task<List<RentalDTO>> GetAllCompletedRentalsAsync()
    {
        var list = await _rentalRepository.GetAllActiveRentalsAssync();
        var listDto = list.ConvertAll(rental =>
            rental.toDTO());
        return listDto;
    }

    public int GetBusyAmmountOfBooks(RentedBookID rentedBookId, RentalStartDate rentalStartDate,
        RentalEndDate rentalEndDate)
    {
        try
        {
            return _rentalRepository.GetBusyAmmountOfBooks(rentedBookId, rentalStartDate, rentalEndDate);
        }
        catch (NullReferenceException ex)
        {
            return 0;
        }
    }
}