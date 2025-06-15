using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Rentals;
using LibraryOnlineRentalSystem.Domain.Book;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace LibraryOnlineRentalSystem.Tests.Domain.Rentals;

[TestFixture]
public class BookRentalIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private string _baseUrl;
    private string _accessToken;
    private string _userEmail;
    private string _userName;
    private string _bookId = "4"; // Predefined book ID for testing
    private string _userPassword;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Create the factory
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _baseUrl = Environment.GetEnvironmentVariable("APP_URL");
        _client.BaseAddress = new Uri(_baseUrl);

        // Generate unique identifiers for the test
        var uniqueId = new string(Guid.NewGuid().ToString("N")
            .Where(char.IsLetter)
            .Take(4)
            .ToArray());
        _userEmail = $"test{uniqueId}@example.com";
        _userName = $"testuser{uniqueId}";
        _userPassword = "TestPassword123!";

        await RegisterUser();
        await LoginUser();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    private async Task RegisterUser()
    {
        var uniquePhoneNumber = new string(Guid.NewGuid().ToString("N")
            .Where(char.IsDigit)
            .Take(7)
            .ToArray());
        var randomNum new string(Guid.NewGuid().ToString("N")
            .Where(char.IsDigit)
            .Take(3)
            .ToArray());
        var newUser = new NewUserDTO
        {
            Name = "Test User",
            Email = _userEmail,
            UserName = _userName,
            Password = _userPassword,
            PhoneNumber = $"91{uniquePhoneNumber}",
            Nif = $"123456{randomNum}",
            Biography = "Test biography"
        };

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
        Assert.That(registerResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), 
            "User registration should succeed");
    }

    private async Task LoginUser()
    {
        var loginRequest = new AuthRequestDTO
        {
            Username = _userName,
            Password = _userPassword
        };

        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
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
        _accessToken = authResponse.AccessToken;

        // Set the authorization header for subsequent requests
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    [Test]
    public async Task CompleteRentalFlow_Success()
    {
        // Arrange
        var rentalRequest = new CreatedRentalDTO(
            startDate: DateTime.UtcNow.AddDays(1).ToString("o"),
            endDate: DateTime.UtcNow.AddDays(7).ToString("o"),
            reservedBookId: _bookId,
            userEmail: _userEmail
        );

        // Act - Create rental
        var rentalContent = new StringContent(
            JsonSerializer.Serialize(rentalRequest),
            Encoding.UTF8,
            "application/json");

        var rentalResponse = await _client.PostAsync("/api/rental", rentalContent);
        
        // Assert - Rental creation
        Assert.That(rentalResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), 
            "Rental creation should succeed");

        // Act - Get user's active rentals
        var userRentalsResponse = await _client.GetAsync($"/api/rental/user/{_userName}/rentals");
        var userRentalsContent = await userRentalsResponse.Content.ReadAsStringAsync();
        var userRentals = JsonSerializer.Deserialize<List<RentalDTO>>(userRentalsContent);

        // Assert - Verify rental in user's rentals
        Assert.That(userRentals, Is.Not.Null, "User rentals should not be null");
        Assert.That(userRentals.Count, Is.GreaterThan(0), "User should have at least one rental");
        var createdRental = userRentals.First(r => r.ReservedBookId == _bookId);
        Assert.That(createdRental.RentalStatus, Is.EqualTo("Active"), 
            "Rental should be in Active status");
    }

    [Test]
    public async Task RentalFlow_InvalidDates_ReturnsBadRequest()
    {
        // Arrange
        var rentalRequest = new CreatedRentalDTO(
            startDate: DateTime.UtcNow.AddDays(7).ToString("o"), // End date before start date
            endDate: DateTime.UtcNow.AddDays(1).ToString("o"),
            reservedBookId: _bookId,
            userEmail: _userEmail
        );

        // Act
        var rentalContent = new StringContent(
            JsonSerializer.Serialize(rentalRequest),
            Encoding.UTF8,
            "application/json");

        var rentalResponse = await _client.PostAsync("/api/rental", rentalContent);
        
        // Assert
        Assert.That(rentalResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), 
            "Rental with invalid dates should fail");
    }

    [Test]
    public async Task RentalFlow_NonExistentBook_ReturnsBadRequest()
    {
        // Arrange
        var rentalRequest = new CreatedRentalDTO(
            startDate: DateTime.UtcNow.AddDays(1).ToString("o"),
            endDate: DateTime.UtcNow.AddDays(7).ToString("o"),
            reservedBookId: "non-existent-book-id",
            userEmail: _userEmail
        );

        // Act
        var rentalContent = new StringContent(
            JsonSerializer.Serialize(rentalRequest),
            Encoding.UTF8,
            "application/json");

        var rentalResponse = await _client.PostAsync("/api/rental", rentalContent);
        
        // Assert
        Assert.That(rentalResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), 
            "Rental with non-existent book should fail");
    }
} 