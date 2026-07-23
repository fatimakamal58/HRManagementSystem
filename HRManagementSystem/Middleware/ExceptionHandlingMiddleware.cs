using HRManagementSystem.Application.Exceptions;

namespace HRManagementSystem.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException exception)
        {
            _logger.LogWarning(
                exception,
                "Requested resource was not found: {Message}",
                exception.Message);

            context.Response.StatusCode =
                StatusCodes.Status404NotFound;

            context.Response.ContentType =
                "text/plain; charset=utf-8";

            await context.Response.WriteAsync(
                "The requested resource was not found.");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unexpected error occurred.");

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            context.Response.ContentType =
                "text/plain; charset=utf-8";

            await context.Response.WriteAsync(
                "An unexpected error occurred.");
        }
    }
}