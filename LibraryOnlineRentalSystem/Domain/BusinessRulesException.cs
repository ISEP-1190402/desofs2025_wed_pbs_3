namespace LibraryOnlineRentalSystem.Domain;

public class BusinessRulesException : Exception
{
    public string Details { get; }

    public BusinessRulesException(string message) : base(message)
    {

    }

    public BusinessRulesException(string message, string details) : base(message)
    {
        Details = details;
    }
}
