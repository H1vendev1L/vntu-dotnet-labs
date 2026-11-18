namespace Pract02.CompanyEmployees.Exceptions;

/// <summary>
/// Власний виняток, що виникає при порушенні бізнес-правил валідації даних співробітника.
/// </summary>
public class EmployeeValidationException : Exception
{
    public string? PropertyName { get; }

    public EmployeeValidationException()
    {
    }

    public EmployeeValidationException(string message)
        : base(message)
    {
    }

    public EmployeeValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public EmployeeValidationException(string message, string propertyName)
        : base(message)
    {
        PropertyName = propertyName;
    }
}