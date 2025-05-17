using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public interface IUserRepository : IRepository<User, UserId>
{
}