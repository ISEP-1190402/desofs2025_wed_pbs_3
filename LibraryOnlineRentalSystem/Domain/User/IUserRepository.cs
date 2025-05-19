namespace LibraryOnlineRentalSystem.Domain.User;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id);

    Task<List<User>> GetAllAsync();

    //void Delete(User user);
}