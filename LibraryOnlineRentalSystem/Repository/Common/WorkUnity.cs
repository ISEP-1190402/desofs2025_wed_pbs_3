using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Repository.Common;

public class WorkUnity : IWorkUnity
{
    private readonly LibraryDbContext _context;

    public WorkUnity(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
}