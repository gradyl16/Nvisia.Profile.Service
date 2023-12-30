namespace Nvisia.Profile.Service.Api.Errors;

public static class ControllerErrors
{
    public static string NotFound(string type) => $"{type} Not Found";
    
    public const string IdGreaterThanZero = "Id Cannot be Negative or Zero";
    
    public const string IdDoesNotMatch = "Id Does Not Match";

    public static string PropertyNotEmpty(string property) => $"'{property}' must not be empty.";
}