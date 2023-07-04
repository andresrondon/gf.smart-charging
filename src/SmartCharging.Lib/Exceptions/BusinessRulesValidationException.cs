namespace SmartCharging.Lib.Exceptions;

public class BusinessRulesValidationException : Exception
{
    public IEnumerable<string> Errors { get; }

    public BusinessRulesValidationException(IEnumerable<string> errors) 
        : base("One or more business rules validation errors occoured.")
    {
        Errors = errors;
        Data["errors"] = Errors;
    }
}
