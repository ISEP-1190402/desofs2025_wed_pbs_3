using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LibraryOnlineRentalSystem.Domain.Common.Interfaces;

namespace LibraryOnlineRentalSystem.Infrastructure.Services
{
    public class EmailOptions
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmail { get; set; } = "examplegogsi@gmail.com";
        public string DevEmail { get; set; } = "examplegogsi@gmail.com"; 
        public bool EnableSsl { get; set; } = true;
    }

    public class DevelopmentEmailService : IEmailService
    {
        private readonly EmailOptions _options;
        private readonly ILogger<DevelopmentEmailService> _logger;

        public DevelopmentEmailService(IOptions<EmailOptions> options, ILogger<DevelopmentEmailService> logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Log configuration values (except password)
            _logger.LogInformation("Email Service initialized with settings: {SmtpServer}:{SmtpPort}, From: {FromEmail}, DevEmail: {DevEmail}", 
                _options.SmtpServer, _options.SmtpPort, _options.FromEmail, _options.DevEmail);
                
            if (string.IsNullOrEmpty(_options.SmtpServer) || 
                _options.SmtpPort <= 0 || 
                string.IsNullOrEmpty(_options.SmtpUsername) || 
                string.IsNullOrEmpty(_options.SmtpPassword))
            {
                _logger.LogWarning("SMTP configuration is incomplete. Email sending may fail.");
            }
        }

        public async Task SendEmailUpdateNotificationAsync(string oldEmail, string newEmail, string username)
        {
            _logger.LogInformation("Sending email update notification for user {Username} from {OldEmail} to {NewEmail}", 
                username, oldEmail, newEmail);
                
            var subject = $"[DEV] Email Update - {username}";
            var body = $@"
            <h2>Email Update Notification (Development Only)</h2>
            <p>User <strong>{username}</strong> has updated their email address.</p>
            <p><strong>Old Email:</strong> {oldEmail}</p>
            <p><strong>New Email:</strong> {newEmail}</p>
            <p>In production, this notification would be sent to: {newEmail}</p>
            <p><em>This is a development notification sent to the developer's email.</em></p>";

            try
            {
                await SendEmailAsync(_options.DevEmail, subject, body);
                _logger.LogInformation("Email update notification sent successfully to {DevEmail}", _options.DevEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email update notification to {DevEmail}", _options.DevEmail);
                throw; // Re-throw to be handled by the caller
            }
        }

        public async Task SendProfileUpdateNotificationAsync(string email, string username, string changes)
        {
            _logger.LogInformation("Sending profile update notification for user {Username} with {ChangeCount} changes", 
                username, changes.Count(c => c == '<')); // Rough count of list items
                
            var subject = $"[DEV] Profile Update - {username}";
            var body = $@"
            <h2>Profile Update Notification (Development Only)</h2>
            <p>User <strong>{username}</strong> has updated their profile with the following changes:</p>
            <ul>{changes}</ul>
            <p>In production, this notification would be sent to: {email}</p>
            <p><em>This is a development notification sent to the developer's email.</em></p>";

            try
            {
                await SendEmailAsync(_options.DevEmail, subject, body);
                _logger.LogInformation("Profile update notification sent successfully to {DevEmail}", _options.DevEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send profile update notification to {DevEmail}", _options.DevEmail);
                throw; // Re-throw to be handled by the caller
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            _logger.LogDebug("Attempting to send email to {ToEmail} with subject: {Subject}", toEmail, subject);
            
            try
            {
                using (var client = new SmtpClient(_options.SmtpServer, _options.SmtpPort))
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_options.FromEmail);
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    _logger.LogDebug("Configuring SMTP client: Server={SmtpServer}, Port={SmtpPort}, SSL={EnableSsl}", 
                        _options.SmtpServer, _options.SmtpPort, _options.EnableSsl);
                        
                    client.EnableSsl = _options.EnableSsl;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_options.SmtpUsername, _options.SmtpPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    
                    // Add timeout and other client settings
                    client.Timeout = 10000; // 10 seconds
                    
                    _logger.LogDebug("Sending email...");
                    await client.SendMailAsync(message);
                    _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error sending email to {ToEmail}. Status: {Status}", 
                    toEmail, smtpEx.StatusCode);
                throw new InvalidOperationException("Failed to send email due to SMTP error", smtpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending email to {ToEmail}", toEmail);
                throw new InvalidOperationException("Failed to send email due to an unexpected error", ex);
            }
        }
    }
}
