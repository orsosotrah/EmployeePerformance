using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.CommonLibrary.Security
{
    public static class SecurityHeaders
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.Add("Content-Security-Policy",
                    "default-src 'self'; " +
                    "img-src 'self' data: https:; " +
                    "font-src 'self' https:; " +
                    "style-src 'self' 'unsafe-inline' https:; " +
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval' https:;");
                context.Response.Headers.Add("Permissions-Policy",
                    "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

                await next();
            });
        }
    }
}