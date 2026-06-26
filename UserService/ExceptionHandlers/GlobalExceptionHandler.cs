using UserService.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Serilog.Core;

namespace UserService.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {

        httpContext.Response.StatusCode = exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

          if (exception is BadRequestException or NotFoundException)
            {
                _logger.LogWarning("Handled client error: {Message}", exception.Message);
            }
            else
            {
                _logger.LogError(exception, "Unhandled server error occurred.");
            }

        await httpContext.Response.WriteAsJsonAsync(new
        {
            Status = httpContext.Response.StatusCode,
            Message = exception.Message
        }, cancellationToken);

        return true;
    }
}