using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class UserName : ICloneable, IValueObject
{
    public UserName() { } 
    public UserName(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
        username=username.Trim();
        if (!username.Any(c => char.IsLetter(c)) || username.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
            throw new BusinessRulesException(
                "The name cannot be alphanumeric or numeric or have special characters except _.");
        if (username.Length > 30) throw new BusinessRulesException("The name cannot be longer than 30 characters.");
        if (username.StartsWith("_") || username.EndsWith("_"))
            throw new BusinessRulesException("The username cannot start or end with special character _.");
        Tag = username;
    }

    public string Tag { get; }

    public object Clone()
    {
        return new UserName(this.Tag);
    }

    public override string ToString()
    {
        return Tag;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not UserName other) return false;

        return Tag.Equals(other.Tag, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Tag.ToLowerInvariant().GetHashCode();
    }
}