using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class Name : ICloneable, IValueObject
{
    public string FullName { get; }

    public Name(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Name cannot be null or empty.");

        name = name.Trim();

        if (name.Any(char.IsDigit))
            throw new BusinessRulesException("The name cannot contain digits.");

        if (name.Any(char.IsSymbol) || name.Any(char.IsPunctuation))
            throw new BusinessRulesException("The name cannot contain special characters.");

        if (name.Length > 40)
            throw new BusinessRulesException("The name cannot exceed 40 characters.");

        FullName = name;
    }

    public object Clone()
    {
        return new Name(this.FullName);
    }

    public override string ToString()
    {
        return FullName;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not Name other) return false;

        return FullName.Equals(other.FullName, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode()
    {
        return FullName.ToLowerInvariant().GetHashCode();
    }
}