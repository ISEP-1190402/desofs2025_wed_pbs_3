using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using LibraryOnlineRentalSystem.Domain.User;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace LibraryOnlineRentalSystem.Tests.Domain.User;

[TestFixture]
public class AuthIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private string _baseUrl;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var keycloakAuthority = Environment.GetEnvironmentVariable("Keycloak__Authority").Replace("\n", "");
        var keycloakAudience = Environment.GetEnvironmentVariable("Keycloak__Audience").Replace("\n", "");
        var keycloakUsername = Environment.GetEnvironmentVariable("Keycloak__Username");
        var keycloakPassword = Environment.GetEnvironmentVariable("Keycloak__Password");
        var libraryDatabase = Environment.GetEnvironmentVariable("LibraryDatabase");

        // Create the factory
        _factory = new WebApplicationFactory<Program>();

        _client = _factory.CreateClient();
        _baseUrl = Environment.GetEnvironmentVariable("APP_URL");
        _client.BaseAddress = new Uri(_baseUrl);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Test]
    public async Task RegisterAndLogin_ValidUser_Success()
    {
        //prepare test data
        var uniqueId = new string(Guid.NewGuid().ToString("N")
            .Where(char.IsDigit)
            .Take(3)
            .ToArray());
        var uniquePhoneNumber = new string(Guid.NewGuid().ToString("N")
            .Where(char.IsDigit)
            .Take(7)
            .ToArray());

        var newUser = new NewUserDTO
        {
            Name = "Test User",
            Email = $"test{uniqueId}@example.com",
            UserName = $"testuser{uniqueId}",
            Password = "TestPassword123!",
            PhoneNumber = $"91{uniquePhoneNumber}",
            Nif = $"123456{uniqueId}",
            Biography = "Test biography"
        };

        // Act - Register
        var registerContent = new StringContent(
            JsonSerializer.Serialize(newUser),
            Encoding.UTF8,
            "application/json");
        
        var registerResponse = await _client.PostAsync("/api/user/register", registerContent);
        
        // Log response content if registration fails
        if (registerResponse.StatusCode != HttpStatusCode.OK)
        {
            var responseContent = await registerResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Registration failed. Status: {registerResponse.StatusCode}");
            Console.WriteLine($"Response content: {responseContent}");
        }
        
        // Assert - Registration
        Assert.That(registerResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), 
            "User registration should succeed");

        // Act - Login
        var loginRequest = new AuthRequestDTO
        {
            Username = newUser.UserName,
            Password = newUser.Password
        };

        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
        
        // Assert - Login
        Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), 
            "Login should succeed");

        var loginResult = await loginResponse.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponseDTO>(loginResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (authResponse == null || string.IsNullOrEmpty(authResponse.TokenType))
        {
            Console.WriteLine("Login response content: " + loginResult);
        }

        Assert.That(authResponse, Is.Not.Null, "Auth response should not be null");
        Assert.That(authResponse.AccessToken, Is.Not.Empty, "Access token should not be empty");
        Assert.That(authResponse.RefreshToken, Is.Not.Empty, "Refresh token should not be empty");
        Assert.That(authResponse.TokenType, Is.EqualTo("Bearer"), "Token type should be Bearer");
        Assert.That(authResponse.ExpiresIn, Is.GreaterThan(0), "Token should have expiration time");
    }

    [Test]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var uniqueId = new string(Guid.NewGuid().ToString("N")
            .Where(char.IsDigit)
            .Take(3)
            .ToArray());
        var uniquePhoneNumber = new string(Guid.NewGuid().ToString("N")
            .Where(char.IsDigit)
            .Take(7)
            .ToArray());

        var newUser = new NewUserDTO
        {
            Name = "Test User",
            Email = $"test{uniqueId}@example.com",
            UserName = $"testuser{uniqueId}",
            Password = "TestPassword123!",
            PhoneNumber = $"91{uniquePhoneNumber}",
            Nif = $"123456{uniqueId}",
            Biography = "Test biography"
        };

        // Act - First registration
        var registerContent = new StringContent(
            JsonSerializer.Serialize(newUser),
            Encoding.UTF8,
            "application/json");
        
        await _client.PostAsync("/api/user/register", registerContent);

        // Act - Second registration with same email
        var duplicateUser = new NewUserDTO
        {
            Name = "Another User",
            Email = newUser.Email, // Same email
            UserName = $"anotheruser{uniqueId}",
            Password = "TestPassword123!",
            PhoneNumber = $"91{uniquePhoneNumber}",
            Nif = $"654321{uniqueId}",
            Biography = "Another biography"
        };

        var duplicateContent = new StringContent(
            JsonSerializer.Serialize(duplicateUser),
            Encoding.UTF8,
            "application/json");
        
        var duplicateResponse = await _client.PostAsync("/api/user/register", duplicateContent);
        
        // Assert
        Assert.That(duplicateResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), 
            "Registration with duplicate email should fail");
    }

    [Test]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
        var loginRequest = new AuthRequestDTO
        {
            Username = "nonexistentuser",
            Password = "wrongpassword"
        };

        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
        
        // Assert
        Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), 
            "Login with invalid credentials should fail");
    }
} 