namespace LibraryOnlineRentalSystem.Domain.Common;

public class BusinessRulesException : Exception
{
    public BusinessRulesException(string message) : base(message)
    {
    }

    public BusinessRulesException(string message, string details) : base(message)
    {
        Details = details;
    }

    public string Details { get; }
}