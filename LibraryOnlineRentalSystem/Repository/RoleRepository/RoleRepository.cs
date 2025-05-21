using LibraryOnlineRentalSystem.Domain.Role;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using LibraryOnlineRentalSystem.Repository.Common;

namespace LibraryOnlineRentalSystem.Repository.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly LibraryDbContext _context;

        public RoleRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
        }
    }
} 