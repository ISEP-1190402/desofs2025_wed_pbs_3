using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public interface IRentalRepository : IRepository<Rental, RentalID>
{
    /// <summary>
    /// Gets a rental by its ID
    /// </summary>
    /// <param name="id">The rental ID</param>
    /// <returns>The rental if found, null otherwise</returns>
    Task<Rental> GetByIdAsync(RentalID id);
    
    Task<List<Rental>> GetAllActiveRentalsAssync();
    Task<List<Rental>> GetAllCancelledRentalsAssync();
    Task<List<Rental>> GetAllPendingRentalsAssync();
    Task<List<Rental>> GetAllCompletedRentalsAssync();
    Task<List<Rental>> GetAllAsyncOfUser(string userEmail);
    Task<List<Rental>> GetAllActiveRentalsOfUserAssync(string userEmail);
    Task<List<Rental>> GetAllCancelledRentalsOfUserAssync(string userEmail);
    Task<List<Rental>> GetAllPendingRentalsOfUserAssync(string userEmail);
    Task<List<Rental>> GetAllCompleteRentalsOfUserAssync(string userEmail);
    Task<Rental> CancelRental(string rentalId);
    int GetBusyAmmountOfBooks(RentedBookID rentedBookId, RentalStartDate rentalStartDate, RentalEndDate rentalEndDate);
    
    Task<List<Rental>> GetOverlappingRentalsAsync(BookID bookId, DateTime startDate, DateTime endDate);
    void Update(Rental rental);
}