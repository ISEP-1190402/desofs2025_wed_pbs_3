namespace LibraryOnlineRentalSystem.Domain.User;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserID id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);

}