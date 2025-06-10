namespace LibraryOnlineRentalSystem.Domain.Common;

public interface IWorkUnity
{
    Task<int> CommitAsync();
}