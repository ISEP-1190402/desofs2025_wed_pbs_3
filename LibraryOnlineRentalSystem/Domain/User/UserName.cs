namespace LibraryOnlineRentalSystem.Domain.User;

public class UserName : ICloneable, IValueObject
{
    public UserName(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
        if (!username.Any(c => char.IsLetter(c)) || username.Any(c => !char.IsLetterOrDigit(c) && c != '_')) throw new BusinessRuleValidationException("The name cannot be alphanumeric or numeric or have special characters except _.");
        if (username.Length > 30) throw new BusinessRuleValidationException("The name cannot be longer than 30 characters.");
        if (username.StartsWith("_") || username.EndsWith("_")) throw new BusinessRuleValidationException("The username cannot start or end with special character _.");
        this.username = username;
    }

    public string username { get; private set; }
    
    public object Clone()
    {
        UserName username = new UserName(this.username);
        return username;
    }

}