using LibraryOnlineRentalSystem.Domain.Role;
using System.Threading.Tasks;

namespace LibraryOnlineRentalSystem.Repository.RoleRepository;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
} 