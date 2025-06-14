using LibraryOnlineRentalSystem.Domain.Common;


namespace LibraryOnlineRentalSystem.Domain.User;

public class User : Entity<UserID>, IAggregateRoot
{
    protected User() { }
    public User(string id, string name, string email, string userName, string phoneNumber, string nif, string biography = "", string hashedPassword = null)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrEmpty(email)) throw new ArgumentException("Email is required", nameof(email));
        if (string.IsNullOrEmpty(userName)) throw new ArgumentException("UserName is required", nameof(userName));
        if (string.IsNullOrEmpty(phoneNumber)) throw new ArgumentException("PhoneNumber is required", nameof(phoneNumber));
        if (string.IsNullOrEmpty(nif)) throw new ArgumentException("Nif is required", nameof(nif));

        Id = new UserID(id);
        Name = new Name(name);
        Email = new Email(email);
        UserName = new UserName(userName);
        PhoneNumber = new PhoneNumber(phoneNumber);
        Nif = new NIF(nif);
        Biography = string.IsNullOrEmpty(biography) ? new Biography() : new Biography(biography);
        HashedPassword = hashedPassword; // This is kept for backward compatibility but won't be used
    }
    


    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public UserName UserName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public NIF Nif { get; private set; }
    public Biography Biography { get; private set; }
    public string HashedPassword { get; private set; }
    
    public void ChangeBiography(string biography)
    {
        Biography = new Biography(biography);
    }

    public void ChangeEmail(string email)
    {
        Email = new Email(email);
    }

    public void ChangePhoneNumber(string phoneNumber)
    {
        PhoneNumber = new PhoneNumber(phoneNumber);
    }

    public void ChangeName(string name)
    {
        Name = new Name(name);
    }

    public void ChangeNif(string nif)
    {
        Nif = new NIF(nif);
    }

    public bool Active { get; private set; } = true;

    public void Deactivate()
    {
        Active = false;
    }
}
