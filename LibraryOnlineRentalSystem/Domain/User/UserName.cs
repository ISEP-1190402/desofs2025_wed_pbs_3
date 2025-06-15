using System;
using System.Linq;
using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class UserName : ICloneable, IValueObject
{
    private static readonly Regex ValidUsernameRegex = new Regex(@"^[a-zA-Z][a-zA-Z0-9_]*$", RegexOptions.Compiled);

    public UserName() { } 
        
    public UserName(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");

        username = username.Trim();

        if (username.Length < 2)
            throw new BusinessRulesException("Username must be at least 2 characters long.");

        if (username.Length > 30)
            throw new BusinessRulesException("Username cannot be longer than 30 characters.");

        if (!ValidUsernameRegex.IsMatch(username))
            throw new BusinessRulesException(
                "Username must start with a letter and can only contain letters, numbers, and underscores.");

        if (username.StartsWith("_") || username.EndsWith("_"))
            throw new BusinessRulesException("Username cannot start or end with an underscore.");

        if (username.Contains("__"))
            throw new BusinessRulesException("Username cannot contain consecutive underscores.");

        Tag = username;
    }


    public string Tag { get; }

    public object Clone()
    {
        return new UserName(Tag);
    }

    public override string ToString()
    {
        return Tag;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not UserName other) return false;
        return string.Equals(Tag, other.Tag, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Tag);
    }
}