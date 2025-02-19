using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.CommonLibrary.Middleware
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
            var sw = Stopwatch.StartNew();

            try
            {
                await LogRequest(context);
                await _next(context);
                await LogResponse(context, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                LogError(context, sw.ElapsedMilliseconds, ex);
                throw;
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            var request = context.Request;

            _logger.LogInformation(
                "HTTP {RequestMethod} {RequestPath} started",
                request.Method,
                request.Path);

            if (ShouldLogRequestBody(request))
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;

                _logger.LogDebug(
                    "Request body: {RequestBody}",
                    body);
            }
        }

        private async Task LogResponse(HttpContext context, long elapsedMs)
        {
            var response = context.Response;

            _logger.LogInformation(
                "HTTP {RequestMethod} {RequestPath} completed in {ElapsedMilliseconds}ms with status code {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                elapsedMs,
                response.StatusCode);
        }

        private void LogError(HttpContext context, long elapsedMs, Exception ex)
        {
            _logger.LogError(
                ex,
                "HTTP {RequestMethod} {RequestPath} failed after {ElapsedMilliseconds}ms",
                context.Request.Method,
                context.Request.Path,
                elapsedMs);
        }

        private bool ShouldLogRequestBody(HttpRequest request)
        {
            return request.ContentType?.StartsWith("application/json") == true ||
                   request.ContentType?.StartsWith("text/") == true;
        }
    }
}