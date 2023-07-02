namespace SmartCharging.Lib.Exceptions;

public class ValidationException : Exception
{
    public IEnumerable<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors) 
        : base("One or more business rules validation errors occoured.")
    {
        Errors = errors;
    }
}
