using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class Email : ICloneable, IValueObject
{
    public string EmailAddress { get; }

    public Email(string email)
    {
        ValidateEmail(email);
        EmailAddress = email;
    }

    private void ValidateEmail(string email)
    {
        var detectPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, detectPattern, RegexOptions.IgnoreCase))
            throw new BusinessRulesException("The email is not valid.");
    }

    public object Clone()
    {
        return new Email(this.EmailAddress);
    }

    public override string ToString()
    {
        return EmailAddress;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not Email other) return false;

        return EmailAddress.ToLowerInvariant() == other.EmailAddress.ToLowerInvariant();
    }

    public override int GetHashCode()
    {
        return EmailAddress.ToLowerInvariant().GetHashCode();
    }
}