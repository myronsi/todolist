using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TodoAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request details
            _logger.LogInformation(
                "Handling request: {Method} {Path}{QueryString} from {RemoteIp}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "",
                context.Connection.RemoteIpAddress?.ToString()
            );

            // Call the next middleware in the pipeline
            await _next(context);

            // Optionally log response details
            _logger.LogInformation(
                "Finished handling request: {StatusCode}",
                context.Response.StatusCode
            );
        }
    }
}