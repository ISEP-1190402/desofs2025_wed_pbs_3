using LibraryOnlineRentalSystem.Domain.Rentals;
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
}