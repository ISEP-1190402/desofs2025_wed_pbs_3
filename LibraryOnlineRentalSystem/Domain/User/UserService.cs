using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using LibraryOnlineRentalSystem.Domain.Common.Interfaces;
using static LibraryOnlineRentalSystem.Controllers.UserController;

namespace LibraryOnlineRentalSystem.Domain.User;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IWorkUnity _workUnit;
    private readonly IAuditLogger _auditLogger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;
    private const string DEFAULT_USER_ROLE_NAME = "User";

    public UserService(IUserRepository userRepository,
        IWorkUnity workUnit,
        IAuditLogger auditLogger,
        HttpClient httpClient,
        IConfiguration configuration,
        IEmailService emailService,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _workUnit = workUnit;
        _auditLogger = auditLogger;
        _httpClient = httpClient;
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task CreateUserAsync(NewUserDTO req)
    {
        _logger.LogInformation("Attempting to create user {UserName} with email {Email}", req.UserName, req.Email);

        if (await _userRepository.GetByEmailAsync(req.Email) != null)
        {
            _logger.LogWarning("Email {Email} already in use", req.Email);

            throw new BusinessRulesException("Email already in use");
        }

        if (await _userRepository.GetByUsernameAsync(req.UserName) != null)
        {
            _logger.LogWarning("Username {Username} already in use", req.UserName);

            throw new BusinessRulesException("Username already in use");
        }

        if (await _userRepository.GetByNifAsync(req.Nif) != null)
        {
            _logger.LogWarning("Nif {Nif} already registered", req.Nif);

            throw new BusinessRulesException("NIF is already registered");
        }

        if (await _userRepository.GetByPhoneNumberAsync(req.PhoneNumber) != null)
        {
            _logger.LogWarning("Phone number {Phone} already in use", req.PhoneNumber);

            throw new BusinessRulesException("Phone number is already in use");
        }

        try
        {
            // First create user in Keycloak
            var adminToken = await GetAdminTokenAsync();
            var keycloakUser = new
            {
                username = req.UserName,
                email = req.Email,
                enabled = true,
                credentials = new[]
                {
                    new
                    {
                        type = "password",
                        value = req.Password,
                        temporary = false
                    }
                },
                requiredActions = new string[] { }
            };

            // Create local user without password (handled by Keycloak)
            var user = new User(
                Guid.NewGuid().ToString(),
                req.Name,
                req.Email,
                req.UserName,
                req.PhoneNumber,
                req.Nif,
                req.Biography,
                hashedPassword: null // No need to store password locally
            );

            var content = new StringContent(
                JsonSerializer.Serialize(keycloakUser),
                Encoding.UTF8,
                "application/json");

            var authority = Environment.GetEnvironmentVariable("Keycloak__Authority")?.TrimEnd('/');
            var keycloakUrl = authority.Replace("/realms/library", "");

            _logger.LogInformation("Sending user creation request to Keycloak...");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.PostAsync($"{keycloakUrl}/admin/realms/library/users", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new BusinessRulesException($"Failed to create user in Keycloak: {error}");
            }

            await Task.Delay(2000);

            string userId = null;
            for (int i = 0; i < 5; i++)
            {
                var getUserResponse =
                    await _httpClient.GetAsync($"{keycloakUrl}/admin/realms/library/users?username={req.UserName}");
                var userContent = await getUserResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Attempt {i + 1} to find user. Response: {userContent}");
                var users = JsonSerializer.Deserialize<List<KeycloakUser>>(userContent);
                if (users != null && users.Count > 0 && !string.IsNullOrEmpty(users[0].Id))
                {
                    userId = users[0].Id;
                    break;
                }

                await Task.Delay(2000);
            }

            if (userId == null)
            {
                throw new BusinessRulesException("Failed to find user in Keycloak after creation.");
            }

            _logger.LogInformation("User {UserName} created successfully in Keycloak with ID {UserId}", req.UserName, userId);

            _httpClient.DefaultRequestHeaders.Authorization = null;

            await _userRepository.AddAsync(user);
            await _workUnit.CommitAsync();
            await _auditLogger.LogAsync($"User {req.UserName} created successfully", "UserCreation");
            _logger.LogInformation("User {UserName} persisted in local database", req.UserName);

        }
        catch (Exception ex)
        {
            throw new BusinessRulesException("Failed to create user: " + ex.Message);
        }
    }

    private async Task<string> GetAdminTokenAsync()
    {
        _logger.LogInformation("Requesting Keycloak admin token...");

        var authority = Environment.GetEnvironmentVariable("Keycloak__Authority")?.TrimEnd('/');
        var keycloakUrl = authority?.Replace("/realms/library", "");

        var username = Environment.GetEnvironmentVariable("Keycloak__Username")?.Trim();
        var password = Environment.GetEnvironmentVariable("Keycloak__Password")?.Trim();

        Console.WriteLine($"[DEBUG] Keycloak__Authority: {authority}");
        Console.WriteLine($"[DEBUG] Keycloak URL: {keycloakUrl}");
        Console.WriteLine($"[DEBUG] Username: {username}");
        Console.WriteLine($"[DEBUG] Password: {(string.IsNullOrEmpty(password) ? "<null or empty>" : "<provided>")}");

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", "admin-cli"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password),
        });

        var response =
            await _httpClient.PostAsync($"{keycloakUrl}/realms/master/protocol/openid-connect/token", content);

        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"[DEBUG] Token response status: {response.StatusCode}");
        Console.WriteLine($"[DEBUG] Token response body: {responseContent}");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get admin token. Response: {Response}", responseContent);

            throw new BusinessRulesException($"Failed to get admin token: {responseContent}");
        }

        var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        return tokenResponse.AccessToken;
    }

    private class KeycloakTokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }
    }

    private class KeycloakUser
    {
        [JsonPropertyName("id")] public string Id { get; set; }

        [JsonPropertyName("username")] public string Username { get; set; }

        [JsonPropertyName("email")] public string Email { get; set; }

        [JsonPropertyName("enabled")] public bool Enabled { get; set; }
    }

    private class KeycloakRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(new UserID(id));
        if (user == null) return null;

        return new UserDTO(
            user.Id.AsGuid(),
            user.Name.FullName,
            user.Email.EmailAddress,
            user.Nif.TaxID,
            user.UserName.Tag,
            user.Biography.Description,
            user.PhoneNumber.Number
        );
    }

    public async Task<UserProfileDTO?> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null) return null;

        return new UserProfileDTO(
            user.Name.FullName,
            user.Email.EmailAddress,
            user.Nif.TaxID,
            user.UserName.Tag,
            user.Biography.Description,
            user.PhoneNumber.Number
        );
    }

    public async Task<List<UserDTO>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(user => new UserDTO(
            user.Id.AsGuid(),
            user.Name.FullName,
            user.Email.EmailAddress,
            user.Nif.TaxID,
            user.UserName.Tag,
            user.Biography.Description,
            user.PhoneNumber.Number
        )).ToList();
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(new UserID(id));
        if (user == null)
            throw new BusinessRulesException("User not found");

        await UpdateUserInternal(user, request, isAdminUpdate: true);
    }

    public async Task UpdateUserByUsernameAsync(string username, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            throw new BusinessRulesException("User not found");

        // Prevent username changes through this endpoint
        if (!string.IsNullOrEmpty(request.UserName) && request.UserName != username)
            throw new BusinessRulesException("You are not allowed to change your username");

        await UpdateUserInternal(user, request, isAdminUpdate: false);
    }

    public async Task UpdateUserByEmailAsync(string email, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new BusinessRulesException("User not found");

        await UpdateUserInternal(user, request);
    }

    protected virtual async Task UpdateUserInternal(User user, UpdateUserRequest request, bool isAdminUpdate = false)
    {
        var changes = new List<string>();
        string oldEmail = user.Email.EmailAddress;
        bool emailChanged = false;

        if (request.Biography != null && request.Biography != user.Biography.Description)
        {
            changes.Add($"<li>Biography updated</li>");
            user.ChangeBiography(request.Biography);
        }

        if (request.PhoneNumber != null && request.PhoneNumber != user.PhoneNumber.Number)
        {
            var existingUserWithPhone = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
            if (existingUserWithPhone != null && existingUserWithPhone.Id.AsString() != user.Id.AsString())
                throw new BusinessRulesException("Phone number is already in use");

            changes.Add($"<li>Phone number changed from {user.PhoneNumber.Number} to {request.PhoneNumber}</li>");
            user.ChangePhoneNumber(request.PhoneNumber);
        }

        if (request.Name != null && request.Name != user.Name.FullName)
        {
            changes.Add($"<li>Name changed from {user.Name.FullName} to {request.Name}</li>");
            user.ChangeName(request.Name);
        }

        if (request.Email != null && request.Email != user.Email.EmailAddress)
        {
            var existingUserWithEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUserWithEmail != null && existingUserWithEmail.Id.AsString() != user.Id.AsString())
                throw new BusinessRulesException("Email is already in use");

            changes.Add($"<li>Email changed from {user.Email.EmailAddress} to {request.Email}</li>");
            user.ChangeEmail(request.Email);
            emailChanged = true;
        }

        if (request.Nif != null && request.Nif != user.Nif.TaxID)
        {
            var existingUserWithNif = await _userRepository.GetByNifAsync(request.Nif);
            if (existingUserWithNif != null && existingUserWithNif.Id.AsString() != user.Id.AsString())
                throw new BusinessRulesException("NIF is already in use");

            changes.Add($"<li>NIF changed from {user.Nif.TaxID} to {request.Nif}</li>");
            user.ChangeNif(request.Nif);
        }

        // Username changes are not allowed
        if (!string.IsNullOrEmpty(request.UserName) && request.UserName != user.UserName.Tag)
        {
            throw new BusinessRulesException("Username cannot be changed");
        }

        if (changes.Any())
        {
            await _workUnit.CommitAsync();
            await _auditLogger.LogAsync($"User {user.Id.AsString()} updated profile with changes: {string.Join(", ", changes)}", "ProfileUpdate");

            // Send email notifications
            try
            {
                if (emailChanged)
                {
                    await _emailService.SendEmailUpdateNotificationAsync(
                        oldEmail,
                        user.Email.EmailAddress,
                        user.UserName.Tag);
                }

                if (changes.Count > 0)
                {
                    await _emailService.SendProfileUpdateNotificationAsync(
                        user.Email.EmailAddress,
                        user.UserName.Tag,
                        string.Join("\n", changes));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email notifications for user {user.Id.AsString()}");
                // Don't fail the request if email sending fails
            }
        }
    }

    public bool UserExists(string userEmail)
    {
        var user = _userRepository.GetByEmailAsync(userEmail).Result;
        return user != null;
    }
}