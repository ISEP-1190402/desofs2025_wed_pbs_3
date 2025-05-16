namespace LibraryOnlineRentalSystem.Domain.User;

public class UserDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Biography { get; set; }

    public Guid RoleId { get; set; }

    public string Nif { get; set; }
    
    public string PhoneNumber { get; set; }

    //public UserDTO() { }


    public UserDTO(Guid Id, string name, string email, Guid roleId, string nif, string username, string biography, string phoneNumber)
    {
        this.Id = Id;
        this.Name = name;
        this.Email = email;
        this.RoleId = roleId;
        this.PhoneNumber = phoneNumber;
        this.Nif = nif;
        this.Biography = biography;
        this.UserName = username;
    }
}
