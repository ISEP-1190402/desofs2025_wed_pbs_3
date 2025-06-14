using LibraryOnlineRentalSystem.Domain.Rentals;
using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace LibraryOnlineRentalSystem.Repository.RentalRepository;

public class RentalRepository : GeneralRepository<Rental, RentalID>,
    IRentalRepository
{
    private readonly LibraryDbContext _context;

    public RentalRepository(LibraryDbContext context) : base(context.Rentals)
    {
        _context = context;
    }

    public async Task<Rental> GetByIdAsync(RentalID id)
    {
        if (id == null) return null;
        
        // Use the base repository method which handles the ID comparison correctly
        return await base.GetByIdAsync(id);
    }

    public async Task<List<Rental>> GetAllActiveRentalsAssync()
    {
        var activeRentals = await _context.Rentals
            .Where(r => r.StatusOfRental.RentStatus == Status.Active)
            .ToListAsync();
        return activeRentals;
    }

    public async Task<List<Rental>> GetAllCancelledRentalsAssync()
    {
        var cancelledRentals = await _context.Rentals
            .Where(r => r.StatusOfRental.RentStatus == Status.Cancelled)
            .ToListAsync();
        return cancelledRentals;
    }

    public async Task<List<Rental>> GetAllPendingRentalsAssync()
    {
        var pendingRentals = await _context.Rentals
            .Where(r => r.StatusOfRental.RentStatus == Status.Pending)
            .ToListAsync();
        return pendingRentals;
    }

    public async Task<List<Rental>> GetAllCompletedRentalsAssync()
    {
        var completedRentals = await _context.Rentals
            .Where(r => r.StatusOfRental.RentStatus == Status.Completed)
            .ToListAsync();
        return completedRentals;
    }

    //USER
    public async Task<List<Rental>> GetAllAsyncOfUser(string userEmail)
    {
        var activeRentals = await _context.Rentals
            .Where(r => r.EmailUser.EmailAddress == userEmail)
            .ToListAsync();
        return activeRentals;
    }

    public async Task<List<Rental>> GetAllActiveRentalsOfUserAssync(string userEmail)
    {
        var activeRentals = await _context.Rentals
            .Where(r => r.StatusOfRental.RentStatus == Status.Active
                        && r.EmailUser.EmailAddress == userEmail)
            .ToListAsync();
        return activeRentals;
    }

    public async Task<List<Rental>> GetAllCancelledRentalsOfUserAssync(string userEmail)
    {
        var cancelledRentals = await _context.Rentals
            .Where(r => r.StatusOfRental.RentStatus == Status.Cancelled
                        && r.EmailUser.EmailAddress == userEmail)
            .ToListAsync();
        return cancelledRentals;
    }

    public async Task<List<Rental>> GetAllPendingRentalsOfUserAssync(string userEmail)
    {
        var pendingRentals = await _context.Rentals
            .Where(r => r.StatusOfRental.RentStatus == Status.Pending
                        && r.EmailUser.EmailAddress == userEmail)
            .ToListAsync();
        return pendingRentals;
    }

    public async Task<List<Rental>> GetAllCompleteRentalsOfUserAssync(string userEmail)
    {
        var completedRentals = await _context.Rentals
            .Where(r => r.StatusOfRental.RentStatus == Status.Completed
                        && r.EmailUser.EmailAddress == userEmail)
            .ToListAsync();
        return completedRentals;
    }

    public async Task<Rental> CancelRental(string rentalId)
    {
        try
        {
            var rental = context().SingleOrDefault(b => b.Id == new RentalID(rentalId));
            if (rental == null)
                return null;

            rental.CancelBooking();

            _context.SaveChanges();
            return rental;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public int GetBusyAmmountOfBooks(RentedBookID rentedBookId, RentalStartDate rentalStartDate,
        RentalEndDate rentalEndDate)
    {
        var completedRentals =  _context.Rentals
            .Where(r => r.RentedBookIdentifier == rentedBookId
                        && r.StartDate.StartDateTime >= rentalStartDate.StartDateTime
                        && r.EndDate.EndDateTime <= rentalEndDate.EndDateTime)
            .ToListAsync();
        return completedRentals.Result.Count();
    }
    
    public async Task<List<Rental>> GetOverlappingRentalsAsync(BookID bookId, DateTime startDate, DateTime endDate)
    {
        // Get all rentals for the specified book that overlap with the given date range
        // An overlap occurs if:
        // 1. Rental starts before or on the end date AND ends on or after the start date
        // 2. Or rental starts within the date range
        // 3. Or rental ends within the date range
        // 4. Or rental completely contains the date range
        var overlappingRentals = await _context.Rentals
            .Where(r => r.RentedBookIdentifier.BookId == bookId.Value &&
                      !(r.StatusOfRental.RentStatus == Status.Cancelled || 
                        r.StatusOfRental.RentStatus == Status.Completed) &&
                      ((r.StartDate.StartDateTime <= endDate && r.EndDate.EndDateTime >= startDate)))
            .ToListAsync();

        return overlappingRentals;
    }
    
    /// <summary>
    /// Updates an existing rental
    /// </summary>
    /// <param name="rental">The rental to update</param>
    public void Update(Rental rental)
    {
        _context.Entry(rental).State = EntityState.Modified;
    }
}