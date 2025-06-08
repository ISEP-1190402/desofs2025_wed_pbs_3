using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public interface IRentalRepository : IRepository<Rental, RentalID>
{
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
}