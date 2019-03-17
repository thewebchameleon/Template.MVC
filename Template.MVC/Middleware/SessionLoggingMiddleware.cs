using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Template.Services.Contracts;

namespace Template.MVC.Middleware
{
    public class SessionLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ISessionService sessionService)
        {
            var session = await sessionService.GetSession();

            // todo: log user actions

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
