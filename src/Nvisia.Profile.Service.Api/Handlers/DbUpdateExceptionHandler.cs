using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Nvisia.Profile.Service.Api.Handlers;

public class DbUpdateExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DefaultExceptionHandler> _logger;
    public DbUpdateExceptionHandler(ILogger<DefaultExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "A database exception occurred while attempting to save entity changes");

        if (exception is DbUpdateException)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = exception.GetType().Name,
                Title = "A database exception occurred while attempting to save entity changes",
                Detail = exception.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            });
            return true;
        }
        return false;
    }
}