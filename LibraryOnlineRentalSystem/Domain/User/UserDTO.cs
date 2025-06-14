namespace LibraryOnlineRentalSystem.Domain.User;

public class UserDTO
{
    public UserDTO() { }

    public UserDTO(Guid id, string name, string email, string nif, string username, string biography,
        string phoneNumber)
    {
        Id = id;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Nif = nif;
        Biography = biography;
        UserName = username;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string UserName { get; private set; }

    public string Email { get; private set; }

    public string Biography { get; private set; }

    public string Nif { get; private set; }

    public string PhoneNumber { get; private set; }
}

public class UserProfileDTO
{
    public UserProfileDTO() { }

    public UserProfileDTO(string name, string email, string nif, string username, string biography,
        string phoneNumber)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Nif = nif;
        Biography = biography;
        UserName = username;
    }

    public string Name { get; private set; }

    public string UserName { get; private set; }

    public string Email { get; private set; }

    public string Biography { get; private set; }

    public string Nif { get; private set; }

    public string PhoneNumber { get; private set; }
}