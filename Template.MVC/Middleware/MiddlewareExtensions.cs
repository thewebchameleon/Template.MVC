using Microsoft.AspNetCore.Builder;

namespace Template.MVC.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionMiddleware>();
        }
    }
}
