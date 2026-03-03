using System.Net;
using System.Text.Json;
using TaskFlow.Middlewares.Exceptions;

namespace TaskFlow.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message = exception.Message;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    break;
                case TaskFlow.Middlewares.Exceptions.BadRequestException:
                    statusCode = StatusCodes.Status400BadRequest;
                    break;
                case TaskFlow.Middlewares.Exceptions.UnauthorizedException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    break;
                case ForbiddenException:
                    statusCode = StatusCodes.Status403Forbidden;
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "Internal Server Error";
                    break;
            }

            var response = new { statusCode = statusCode, message = message };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
