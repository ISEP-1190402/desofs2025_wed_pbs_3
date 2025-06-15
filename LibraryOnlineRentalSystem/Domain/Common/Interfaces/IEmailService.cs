using System.Threading.Tasks;

namespace LibraryOnlineRentalSystem.Domain.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailUpdateNotificationAsync(string oldEmail, string newEmail, string username);
        Task SendProfileUpdateNotificationAsync(string email, string username, string changes);
    }
}
