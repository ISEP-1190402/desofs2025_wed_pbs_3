namespace LibraryOnlineRentalSystem.Domain.User;

public class NewUserDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string Nif { get; set; }
    public string Biography { get; set; }
    public string Password { get; set; }
    
    public NewUserDTO() { }
    
    public NewUserDTO(string name, string email, string roleId, string userName, string phoneNumber, string nif, string biography, string password)
    {
        Name = name;
        Email = email;
        UserName = userName;
        PhoneNumber = phoneNumber;
        Nif = nif;
        Biography = biography;
        Password = password;
    }
}

