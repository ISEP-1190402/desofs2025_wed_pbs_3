namespace LibraryOnlineRentalSystem.Domain.Common;

public interface IAuditLogger
{
    Task LogAsync(string message, string category);
}