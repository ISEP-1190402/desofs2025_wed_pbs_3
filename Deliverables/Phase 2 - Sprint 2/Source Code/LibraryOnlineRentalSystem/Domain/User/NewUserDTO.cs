namespace LibraryOnlineRentalSystem.Domain.User;

public class NewUserDTO
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Nif { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public string Password { get; set; } = string.Empty;
    
    public NewUserDTO() { }
    
    public NewUserDTO(string name, string email, string userName, string phoneNumber, string nif, string? biography = null, string password = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Nif = nif ?? throw new ArgumentNullException(nameof(nif));
        Biography = biography;
        Password = password ?? throw new ArgumentNullException(nameof(password));
    }
}

