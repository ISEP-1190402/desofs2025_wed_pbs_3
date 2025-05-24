using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Role;
using static LibraryOnlineRentalSystem.Controllers.UserController;
using LibraryOnlineRentalSystem.Repository.RoleRepository;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

namespace LibraryOnlineRentalSystem.Domain.User;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IWorkUnity _workUnit;
    private readonly IAuditLogger _auditLogger;
    private readonly IRoleRepository _roleRepository;
    private readonly PasswordService _passwordService;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private const string DEFAULT_USER_ROLE_NAME = "User";

    public UserService(
        IUserRepository userRepository,
        IWorkUnity workUnit,
        IAuditLogger auditLogger,
        IRoleRepository roleRepository,
        PasswordService passwordService,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _workUnit = workUnit;
        _auditLogger = auditLogger;
        _roleRepository = roleRepository;
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

        var userRole = await _roleRepository.GetByNameAsync(DEFAULT_USER_ROLE_NAME);
        if (userRole == null)
            throw new BusinessRulesException("Default user role not found");

        try
        {
            // Get admin token
            var adminToken = await GetAdminTokenAsync();

            // Create user in Keycloak
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
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(keycloakUser),
                Encoding.UTF8,
                "application/json");

            var authority = _configuration["Keycloak:Authority"].TrimEnd('/');
            var keycloakUrl = authority.Replace("/realms/library", "");
            
            // Add Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
            
            var response = await _httpClient.PostAsync($"{keycloakUrl}/admin/realms/library/users", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new BusinessRulesException($"Failed to create user in Keycloak: {error}");
            }

            // After creating the user in Keycloak
            await Task.Delay(1000); // Wait 1 second

            // Retry fetching user ID from Keycloak
            string userId = null;
            for (int i = 0; i < 5; i++) {
                var getUserResponse = await _httpClient.GetAsync($"{keycloakUrl}/admin/realms/library/users?username={req.UserName}");
                var userContent = await getUserResponse.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<KeycloakUser>>(userContent);
                if (users != null && users.Count > 0 && !string.IsNullOrEmpty(users[0].Id)) {
                    userId = users[0].Id;
                    break;
                }
                await Task.Delay(1000); // wait 1 second before retry
            }
            if (userId == null) {
                throw new BusinessRulesException("Failed to find user in Keycloak after creation.");
            }

            // Get the User role ID
            var getRoleResponse = await _httpClient.GetAsync($"{keycloakUrl}/admin/realms/library/roles/User");
            var roleResponseContent = await getRoleResponse.Content.ReadAsStringAsync();
            var role = JsonSerializer.Deserialize<KeycloakRole>(roleResponseContent);

            // Assign User role
            var roleUrl = $"{keycloakUrl}/admin/realms/library/users/{userId}/role-mappings/realm";
            var roleData = new[]
            {
                new
                {
                    id = role.Id,
                    name = "User",
                    description = "Regular user role"
                }
            };

            var roleAssignmentContent = new StringContent(
                JsonSerializer.Serialize(roleData),
                Encoding.UTF8,
                "application/json");

            var roleAssignmentResponse = await _httpClient.PostAsync(roleUrl, roleAssignmentContent);
            if (!roleAssignmentResponse.IsSuccessStatusCode)
            {
                var error = await roleAssignmentResponse.Content.ReadAsStringAsync();
                throw new BusinessRulesException($"Failed to assign role: {error}");
            }

            // Clear Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Hash the password
            var hashedPassword = _passwordService.HashPassword(req.Password);

            // Create user in our database with hashed password
            var user = new User(
                Guid.NewGuid().ToString(),
                req.Name,
                req.Email,
                userRole.Id.AsString(),
                req.UserName,
                req.PhoneNumber,
                req.Nif,
                req.Biography,
                hashedPassword
            );

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

        var response = await _httpClient.PostAsync($"{keycloakUrl}/realms/master/protocol/openid-connect/token", content);
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
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }

    private class KeycloakUser
    {
        public string Id { get; set; }
    }

    private class KeycloakRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(id));
        if (user == null) return null;

        return new UserDTO(
            user.Id.AsGuid(),
            user.Name.FullName,
            user.Email.EmailAddress,
            user.RoleId.AsGuid(),
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
            user.RoleId.AsGuid(),
            user.Nif.TaxID,
            user.UserName.Tag,
            user.Biography.Description,
            user.PhoneNumber.Number
        )).ToList();
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(id));
        if (user == null)
            throw new BusinessRulesException("User not found");

        if (request.Biography != null)
            user.ChangeBiography(request.Biography);

        if (request.PhoneNumber != null)
            user.ChangePhoneNumber(request.PhoneNumber);

        if (request.RoleId != null)
            user.ChangeRoleId(request.RoleId);

        if (request.Name != null)
            user.ChangeName(request.Name);

        if (request.Email != null)
            user.ChangeEmail(request.Email);

        await _workUnit.CommitAsync();
        await _auditLogger.LogAsync($"User {id} updated profile.", "ProfileUpdate");
    }

}
