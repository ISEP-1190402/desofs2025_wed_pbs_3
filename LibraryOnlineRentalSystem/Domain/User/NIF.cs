using LibraryOnlineRentalSystem.Domain.Shared;

namespace LibraryOnlineRentalSystem.Domain.User;

public class NIF : ICloneable, IValueObject
{
    public NIF(string nif)
    {
        if (string.IsNullOrEmpty(nif))
            throw new ArgumentNullException(nameof(nif));

        nif = nif.Trim();

        if (nif.Length != 9 || !nif.All(char.IsDigit))
            throw new BusinessRulesException("The NIF must contain exactly 9 digits.");

        this.nif = nif;
    }

    public string nif { get; private set; }

    public object Clone()
    {
        return new NIF(this.nif);
    }
}