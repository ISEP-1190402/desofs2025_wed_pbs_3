using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace LibraryOnlineRentalSystem.Domain.User;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _tokenEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public AuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        
        var authority = _configuration["Keycloak:Authority"];
        if (string.IsNullOrEmpty(authority))
            throw new ArgumentException("Keycloak:Authority is not configured");

        // Remove trailing slash if present
        authority = authority.TrimEnd('/');
        _tokenEndpoint = $"{authority}/protocol/openid-connect/token";
        _clientId = _configuration["Keycloak:ClientId"];
        _clientSecret = _configuration["Keycloak:ClientSecret"];
    }

    public async Task<AuthResponseDTO> AuthAsync(AuthRequestDTO request)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret),
            new KeyValuePair<string, string>("username", request.Username),
            new KeyValuePair<string, string>("password", request.Password)
        });

        var response = await _httpClient.PostAsync(_tokenEndpoint, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Raw response from Keycloak: {responseContent}"); //to be removed after testing
        var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(responseContent);

        return new AuthResponseDTO
        {
            AccessToken = tokenResponse.access_token,
            RefreshToken = tokenResponse.refresh_token,
            ExpiresIn = tokenResponse.expires_in,
            TokenType = tokenResponse.token_type
        };
    }

    public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret),
            new KeyValuePair<string, string>("refresh_token", refreshToken)
        });

        var response = await _httpClient.PostAsync(_tokenEndpoint, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Raw response from Keycloak: {responseContent}"); //to be removed after testing
        var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(responseContent);

        return new AuthResponseDTO
        {
            AccessToken = tokenResponse.access_token,
            RefreshToken = tokenResponse.refresh_token,
            ExpiresIn = tokenResponse.expires_in,
            TokenType = tokenResponse.token_type
        };
    }

    private class KeycloakTokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
} 