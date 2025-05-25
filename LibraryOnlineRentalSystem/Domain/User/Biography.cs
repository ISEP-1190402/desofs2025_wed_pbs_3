using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class Biography : ICloneable, IValueObject
{
    public Biography(string biography)
    {
        if (string.IsNullOrWhiteSpace(biography))
        {
            this.biography = string.Empty;
            return;
        }

        biography = biography.Trim();

        if (biography.Length > 150)
            throw new BusinessRulesException("Description cannot exceed 150 characters.");

        if (!biography.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
            throw new BusinessRulesException("Biography cannot contain emojis or special characters.");

        this.biography = biography;
    }

    public string biography { get; }

    public object Clone()
    {
        var biography = new Biography(this.biography);
        return biography;
    }
}