using LibraryOnlineRentalSystem.Domain.Shared;

namespace LibraryOnlineRentalSystem.Domain.User;

public class PhoneNumber : ICloneable, IValueObject
{
    public string Number { get; private set; }

    public PhoneNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentNullException(nameof(number));

        number = number.Trim();
        
        if (number.Length != 9 || !number.All(char.IsDigit))
            throw new BusinessRulesException("The phone number must contain exactly 9 digits, without country code +351.");
        
        Number = number;
    }

    public object Clone()
    {
        return new PhoneNumber(this.Number);
    }
}
