using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.User;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentalService
{
    private readonly IRentalRepository _rentalRepository;

    private readonly IWorkUnity _workUnity;

    public RentalService(IRentalRepository rentalRepository, IWorkUnity workUnity)
    {
        _rentalRepository = rentalRepository;
        _workUnity = workUnity;
    }


    // USER - Rent a Book

    public async Task<RentalDTO> CreateARentalAsync(CreatedRentalDTO rental)
    {
        var id = _rentalRepository.GetAllAsync().Result.Count + 1 + "";

        var addedRental = new Rental(id, rental.StartDate, rental.EndDate, rental.ReservedBookId, rental.UserEmail);

        await _rentalRepository.AddAsync(addedRental);

        await _workUnity.CommitAsync();

        return addedRental.toDTO();
    }

    // USER - Cancel a Rental

    public async Task<RentalDTO> CancelARental(string rentalID)
    {
        var rental = _rentalRepository.CancelRental(rentalID);
        await _workUnity.CommitAsync();

        return rental.Result.toDTO();
    }

    // USER - GET ALL Rentals
    public async Task<List<RentalDTO>> GetAllRentalsOfUserAsync(string userEmail)
    {
        var list = await _rentalRepository.GetAllAsyncOfUser(userEmail);
        var listDto = list.ConvertAll(rental =>
            rental.toDTO());
        return listDto;
    }

    // USER - GET Active Rentals
    public async Task<List<RentalDTO>> GetAllActiveRentalsOfUserAsync(string userEmail)
    {
        var list = await _rentalRepository.GetAllActiveRentalsOfUserAssync(userEmail);
        var listDto = list.ConvertAll(rental =>
            rental.toDTO());
        return listDto;
    }

    // USER - GET Cancelled Rentals
    public async Task<List<RentalDTO>> GetAllCancelledRentalsOfUserAsync(string userEmail)
    {
        var list = await _rentalRepository.GetAllCancelledRentalsOfUserAssync(userEmail);
        var listDto = list.ConvertAll(rental =>
            rental.toDTO());
        return listDto;
    }

    // USER - GET Pending Rentals
    public async Task<List<RentalDTO>> GetAllPendingRentalsOfUserAsync(string userEmail)
    {
        var list = await _rentalRepository.GetAllPendingRentalsOfUserAssync(userEmail);
        var listDto = list.ConvertAll(rental =>
            rental.toDTO());
        return listDto;
    }

    // Library Manager - GET Completed Rentals
    public async Task<List<RentalDTO>> GetAllCompletedRentalsOfUserAsync(string userEmail)
    {
        var list = await _rentalRepository.GetAllCompleteRentalsOfUserAssync(userEmail);
        var listDto = list.ConvertAll(rental =>
            rental.toDTO());
        return listDto;
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
        return  _rentalRepository.GetBusyAmmountOfBooks(rentedBookId, rentalStartDate, rentalEndDate);
    }
}