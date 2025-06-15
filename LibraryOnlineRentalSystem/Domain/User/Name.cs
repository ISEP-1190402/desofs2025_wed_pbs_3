using System;
using System.Linq;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class Name : ICloneable, IValueObject
{
    public Name() { } 
    
    public Name(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Name cannot be null or empty.");

        name = name.Trim();

        if (name.Length < 1)
            throw new BusinessRulesException("Name must be at least 1 character long.");

        if (name.Length > 40)
            throw new BusinessRulesException("Name cannot be longer than 40 characters.");

        if (name.Any(char.IsDigit))
            throw new BusinessRulesException("Name cannot contain digits.");

        if (name.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c) && c != '-' && c != '\''))
            throw new BusinessRulesException("Name can only contain letters, spaces, hyphens, and apostrophes.");

        FullName = name;
    }

    public string FullName { get; }

    public object Clone()
    {
        return new Name(FullName);
    }

    public override string ToString()
    {
        return FullName;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not Name other) return false;
        return string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(FullName);
    }
}