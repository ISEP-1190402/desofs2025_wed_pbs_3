using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class PhoneNumber : ICloneable, IValueObject
{
    public PhoneNumber() { } 
    public PhoneNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentNullException(nameof(number), "Phone number cannot be null or empty.");

        number = number.Trim();

        if (number.Length != 9 || !number.All(char.IsDigit))
            throw new BusinessRulesException("The phone number must contain exactly 9 digits, without country code +351.");

 
        if (!Regex.IsMatch(number, @"^(9[1236]\d{7}|2\d{8}|3[123589]\d{7})$"))
            throw new BusinessRulesException("Invalid Portuguese phone number format.");

        Number = number;
    }

    public string Number { get; }

    public object Clone()
    {
        return new PhoneNumber(Number);
    }

    public override string ToString()
    {
        return Number;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not PhoneNumber other) return false;
        return Number == other.Number;
    }

    public override int GetHashCode()
    {
        return Number?.GetHashCode() ?? 0;
    }
}