using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.CommonLibrary.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }

        public static IApplicationBuilder UseCorrelation(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationMiddleware>();
        }

        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}