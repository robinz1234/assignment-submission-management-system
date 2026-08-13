using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException exception)
        {
            await WriteProblemAsync(context, exception.StatusCode, exception.Message, exception.Details);
        }
        catch (UnauthorizedAccessException exception)
        {
            await WriteProblemAsync(context, StatusCodes.Status401Unauthorized, exception.Message);
        }
        catch (DbUpdateException exception)
        {
            logger.LogWarning(exception, "A database update conflict occurred.");
            await WriteProblemAsync(
                context,
                StatusCodes.Status409Conflict,
                "The operation conflicts with existing data.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unhandled server error occurred.");
            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected server error occurred.");
        }
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string message,
        object? details = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = message,
            Instance = context.Request.Path
        };

        if (details is not null)
        {
            problem.Extensions["details"] = details;
        }

        await context.Response.WriteAsJsonAsync(problem);
    }
}
