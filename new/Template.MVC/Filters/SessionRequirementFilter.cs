using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Template.Services.Contracts;

namespace Template.MVC.Filters
{
    /// <summary>
    /// this class ensures that an authenticated user has a session when they are logging in (in-case the application state is restarted)
    /// </summary>
    public class SessionRequirementFilter : IAsyncAuthorizationFilter
    {
        private readonly ISessionService _sessionService;

        public SessionRequirementFilter(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var session = await _sessionService.GetSession();
            if (context.HttpContext.User.Identity.IsAuthenticated && session.User == null)
            {
                await context.HttpContext.SignOutAsync();
                context.Result = new ChallengeResult();
            }
        }
    }
}
