using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using WorkoutTrackerApi.Application;

namespace WorkoutTrackerApi.Controllers;

public class NotFoundExceptionHandler : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken) {
        if (exception is not NotFoundException) 
            return false;

        context.Response.StatusCode = StatusCodes.Status404NotFound;

        await context.Response.WriteAsJsonAsync(
            new {
                error = exception.Message
            }, 
            cancellationToken
        );

        return true;
    }
}

public class InvalidOperationExceptionHandler : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken) {
        if (exception is not InvalidOperationException) 
            return false;

        context.Response.StatusCode = StatusCodes.Status409Conflict;

        await context.Response.WriteAsJsonAsync(
            new {
                error = exception.Message
            }, 
            cancellationToken
        );

        return true;
    }
}

public class UnauthorizedAccessExceptionHandler : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken) {
        if (exception is not UnauthorizedAccessException) 
            return false;

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await context.Response.WriteAsJsonAsync(
            new {
                error = exception.Message
            }, 
            cancellationToken
        );

        return true;
    }
}

public class ForbiddenExceptionHandler : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken) {
        if (exception is not ForbiddenException) 
            return false;

        context.Response.StatusCode = StatusCodes.Status403Forbidden;

        await context.Response.WriteAsJsonAsync(
            new {
                error = exception.Message
            }, 
            cancellationToken
        );

        return true;
    }
}

public class ValidationExceptionHandler : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken) {
        if (exception is not ValidationException) 
            return false;

        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        await context.Response.WriteAsJsonAsync(
            new {
                error = exception.Message
            }, 
            cancellationToken
        );

        return true;
    }
}