using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Template.Services.Contracts;

namespace Template.MVC.Middleware
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ISessionService sessionService)
        {
            // calling this method ensures a session is created if one does not already exist.
            var session = await sessionService.GetSession();

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
