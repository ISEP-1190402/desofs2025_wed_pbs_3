using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class Biography : ICloneable, IValueObject
{
    public Biography() { } 
    public Biography(string biography)
    {
        if (string.IsNullOrWhiteSpace(biography))
        {
            this.Description = string.Empty;
            return;
        }

        biography = biography.Trim();

        if (biography.Length > 150)
            throw new BusinessRulesException("Description cannot exceed 150 characters.");

        if (!biography.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
            throw new BusinessRulesException("Biography cannot contain emojis or special characters.");

        Description = biography;
    }

    public string Description { get; }

    public object Clone()
    {
        return new Biography(this.Description);
    }

    public override string ToString()
    {
        return Description;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not Biography other) return false;

        return Description.Equals(other.Description, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Description.ToLowerInvariant().GetHashCode();
    }
}