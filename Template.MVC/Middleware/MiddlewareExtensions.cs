using Microsoft.AspNetCore.Builder;

namespace Template.MVC.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionLoggingMiddleware>();
        }
    }
}
