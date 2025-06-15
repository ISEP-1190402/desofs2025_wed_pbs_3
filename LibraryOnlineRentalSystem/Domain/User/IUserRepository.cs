namespace LibraryOnlineRentalSystem.Domain.User;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserID id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByNifAsync(string nif);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
}