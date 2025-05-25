namespace LibraryOnlineRentalSystem.Domain.User;

public class AuthRequestDTO
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class AuthResponseDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; }
}

public class RefreshTokenRequestDTO
{
    public string RefreshToken { get; set; }
} 