using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class Email : ICloneable, IValueObject
{
    public Email() { } 
    public Email(string email)
    {
        email = email.Trim();
        ValidateEmail(email);
        EmailAddress = email;
    }
    public string EmailAddress { get; }

    private void ValidateEmail(string email)
    {
        var detectPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, detectPattern, RegexOptions.IgnoreCase))
            throw new BusinessRulesException("The email is not valid.");
        if (email.Contains(".."))
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

        return EmailAddress.ToLower() == other.EmailAddress.ToLower();
    }

    public override int GetHashCode()
    {
        return EmailAddress.ToLowerInvariant().GetHashCode();
    }
}