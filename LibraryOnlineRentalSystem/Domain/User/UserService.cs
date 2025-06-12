using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using static LibraryOnlineRentalSystem.Controllers.UserController;

namespace LibraryOnlineRentalSystem.Domain.User;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IWorkUnity _workUnit;
    private readonly IAuditLogger _auditLogger;
    private readonly PasswordService _passwordService;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private const string DEFAULT_USER_ROLE_NAME = "User";

    public UserService(
        IUserRepository userRepository,
        IWorkUnity workUnit,
        IAuditLogger auditLogger,
        PasswordService passwordService,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _workUnit = workUnit;
        _auditLogger = auditLogger;
        _passwordService = passwordService;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task CreateUserAsync(NewUserDTO req)
    {
        if (await _userRepository.GetByEmailAsync(req.Email) != null)
            throw new BusinessRulesException("Email already in use");

        if (await _userRepository.GetByUsernameAsync(req.UserName) != null)
            throw new BusinessRulesException("Username already in use");

        try
        {
            // Validate user data before creating in Keycloak
            var user = new User(
                Guid.NewGuid().ToString(),
                req.Name,
                req.Email,
                req.UserName,
                req.PhoneNumber,
                req.Nif,
                req.Biography,
                _passwordService.HashPassword(req.Password)
            );

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

            var content = new StringContent(
                JsonSerializer.Serialize(keycloakUser),
                Encoding.UTF8,
                "application/json");

            var authority = _configuration["Keycloak:Authority"].TrimEnd('/');
            var keycloakUrl = authority.Replace("/realms/library", "");

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

            _httpClient.DefaultRequestHeaders.Authorization = null;

            await _userRepository.AddAsync(user);
            await _workUnit.CommitAsync();
            await _auditLogger.LogAsync($"User {req.UserName} created successfully", "UserCreation");
        }
        catch (Exception ex)
        {
            throw new BusinessRulesException("Failed to create user: " + ex.Message);
        }
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var authority = _configuration["Keycloak:Authority"].TrimEnd('/');
        var keycloakUrl = authority.Replace("/realms/library", "");
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", "admin-cli"),
            new KeyValuePair<string, string>("username", "admin"),
            new KeyValuePair<string, string>("password", "admin")
        });

        var response =
            await _httpClient.PostAsync($"{keycloakUrl}/realms/master/protocol/openid-connect/token", content);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BusinessRulesException($"Failed to get admin token: {error}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
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

        if (request.Biography != null)
            user.ChangeBiography(request.Biography);

        if (request.PhoneNumber != null)
            user.ChangePhoneNumber(request.PhoneNumber);

        if (request.Name != null)
            user.ChangeName(request.Name);

        if (request.Email != null)
            user.ChangeEmail(request.Email);

        await _workUnit.CommitAsync();
        await _auditLogger.LogAsync($"User {id} updated profile.", "ProfileUpdate");
    }

    public bool UserExists(string userEmail)
    {
        var user = _userRepository.GetByEmailAsync(userEmail).Result;
        return user != null;
    }
}