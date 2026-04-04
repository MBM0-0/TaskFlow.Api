using System.Net;
using System.Security.Claims;
using System.Text.Json;
using TaskFlow.Extensions.Middlewares.Exceptions;

namespace TaskFlow.Extensions.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            int statusCode;
            string message = exception.Message;
            
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var path = context.Request.Path;
            var userName = context.User?.Identity?.IsAuthenticated == true? context.User.FindFirst(ClaimTypes.Name) ?.Value ?? "unknown": "anonymous";

            switch (exception)
            {
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    break;
                case BadRequestException:
                    statusCode = StatusCodes.Status400BadRequest;
                    break;
                case UnauthorizedException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    logger.LogWarning($"Unauthorized access | User: {userName} | IP: {ip} | Path: {path}");

                    break;
                case ForbiddenException:
                    statusCode = StatusCodes.Status403Forbidden;
                    logger.LogWarning($"Forbidden access | User: {userName} | IP: {ip} | Path: {path}");
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "Internal Server Error";
                    logger.LogError(exception, $"Server error | User: {userName} | IP: {ip} | Path: {path}");
                    break;
            }

            var response = new { statusCode = statusCode, message = message };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
