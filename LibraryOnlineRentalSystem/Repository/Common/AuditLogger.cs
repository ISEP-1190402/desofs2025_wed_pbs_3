using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Repository.Common;

public class AuditLogger : IAuditLogger
{
    public Task LogAsync(string message, string category)
    {
        Console.WriteLine($"[{category}] {message}");
        return Task.CompletedTask;
    }
} 