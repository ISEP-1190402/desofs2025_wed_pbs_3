using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class Name : ICloneable, IValueObject
{
    public Name(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        name = name.Trim();
        if (name.Any(char.IsDigit) || name.Any(char.IsNumber))
            throw new BusinessRulesException("The name cannot be alphanumeric or numeric.");
        if (name.Any(char.IsSymbol) || name.Any(char.IsPunctuation))
            throw new BusinessRulesException("The name cannot have special characters.");
        if (name.Length > 60) throw new BusinessRulesException("The name cannot have more than 60 characters.");
        this.name = name;
    }

    public string name { get; }

    public object Clone()
    {
        var name = new Name(this.name);
        return name;
    }
}