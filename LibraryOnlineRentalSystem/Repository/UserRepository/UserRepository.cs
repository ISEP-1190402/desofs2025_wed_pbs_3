using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace LibraryOnlineRentalSystem.Repository.UserRepository;

public class UserRepository : GeneralRepository<User, UserId>, IUserRepository
{
    private readonly LibraryDbContext _context;

    public UserRepository(LibraryDbContext context) : base(context.Users)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(UserId id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email.email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName.username == username);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public void Delete(User user)
    {
        _context.Users.Remove(user);
    }
}
