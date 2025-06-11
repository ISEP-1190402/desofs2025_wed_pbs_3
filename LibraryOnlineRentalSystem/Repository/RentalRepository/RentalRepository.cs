using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Rentals;
using Microsoft.EntityFrameworkCore;
using LibraryOnlineRentalSystem.Repository.Common;

namespace LibraryOnlineRentalSystem.Repository.RentalRepository;

public class RentalRepository : IRentalRepository
{
    private readonly LibraryDbContext _context;
    private readonly DbSet<Rental> _rentals;

    public RentalRepository(LibraryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _rentals = context.Set<Rental>();
    }

    public async Task<Rental?> GetByIdAsync(RentalID id)
    {
        if (id == null || string.IsNullOrWhiteSpace(id.Value))
            throw new ArgumentException("Rental ID cannot be null or empty", nameof(id));

        return await _rentals
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Rental>> GetAllAsync()
    {
        return await _rentals.ToListAsync();
    }

    public async Task<List<Rental>> GetAllAsyncOfUser(string userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email cannot be empty", nameof(userEmail));

        return await _rentals
            .Where(r => r.EmailUser.EmailAddress == userEmail)
            .ToListAsync();
    }
    

    public async Task<List<Rental>> GetAllActiveRentalsAssync()
    {
        return await _rentals
            .Where(r => r.StatusOfRental.IsActive)
            .ToListAsync();
    }

    public async Task<List<Rental>> GetAllPendingRentalsAssync()
    {
        return await _rentals
            .Where(r => r.StatusOfRental.IsPending)
            .ToListAsync();
    }

    public async Task<List<Rental>> GetAllCompletedRentalsAssync()
    {
        return await _rentals
            .Where(r => r.StatusOfRental.IsCompleted)
            .ToListAsync();
    }

    public async Task<List<Rental>> GetAllCancelledRentalsAssync()
    {
        return await _rentals
            .Where(r => r.StatusOfRental.IsCancelled)
            .ToListAsync();
    }

    public async Task<List<Rental>> GetAllCompletedRentalsOfUserAssync(string userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email cannot be empty", nameof(userEmail));

        return await _rentals
            .Where(r => r.EmailUser.EmailAddress == userEmail && r.StatusOfRental.IsCompleted)
            .ToListAsync();
    }

    public async Task<Rental> AddAsync(Rental entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _rentals.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task<List<Rental>> GetAllActiveRentalsOfUserAssync(string userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email cannot be empty", nameof(userEmail));

        return await _rentals
            .Where(r => r.EmailUser.EmailAddress == userEmail && r.StatusOfRental.IsActive)
            .ToListAsync();
    }

    public async Task<List<Rental>> GetAllCancelledRentalsOfUserAssync(string userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email cannot be empty", nameof(userEmail));

        return await _rentals
            .Where(r => r.EmailUser.EmailAddress == userEmail && r.StatusOfRental.IsCancelled)
            .ToListAsync();
    }

    public async Task<List<Rental>> GetAllPendingRentalsOfUserAssync(string userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email cannot be empty", nameof(userEmail));

        return await _rentals
            .Where(r => r.EmailUser.EmailAddress == userEmail && r.StatusOfRental.IsPending)
            .ToListAsync();
    }

    public void Update(Rental entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _rentals.Update(entity);
    }

    public void Remove(Rental entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _rentals.Remove(entity);
    }

    public async Task<bool> CancelRentalAsync(string rentalId)
    {
        if (string.IsNullOrWhiteSpace(rentalId))
            throw new ArgumentException("Rental ID cannot be empty", nameof(rentalId));

        var rental = await GetByIdAsync(new RentalID(rentalId));
        if (rental == null)
            return false;

        rental.CancelBooking();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteRentalAsync(string rentalId)
    {
        if (string.IsNullOrWhiteSpace(rentalId))
            throw new ArgumentException("Rental ID cannot be empty", nameof(rentalId));

        var rental = await GetByIdAsync(new RentalID(rentalId));
        if (rental == null)
            return false;

        rental.MarkAsCompleted();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("A concurrency error occurred while saving to the database.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("An error occurred while saving to the database.", ex);
        }
    }
}