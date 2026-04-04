using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

namespace TaskFlow.Extensions
{
    public static class RateLimiterExtensions
    {
        public static void AddTaskFlowRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                 {
                     var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("RateLimitLogger");
                     var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                     logger.LogWarning($"Rate limit exceeded by IP: {ip}");

                     await context.HttpContext.Response.WriteAsync("Too many requests. Try again later.");
                 };

                options.AddPolicy("AuthPolicy", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey: ip, factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });
            });
        }
    }
}
