namespace AssignmentManagement.Api.Middleware;

public class ApiException(int statusCode, string message, object? details = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public object? Details { get; } = details;
}
