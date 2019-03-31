using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using Template.Services.Contracts;

namespace Template.MVC.Filters
{
    public class PermissionRequirementFilter : IAsyncAuthorizationFilter
    {
        private readonly string _key;
        private readonly ISessionService _sessionService;

        public PermissionRequirementFilter(string key, ISessionService sessionService)
        {
            _key = key;
            _sessionService = sessionService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var session = await _sessionService.GetSession();

            if (session.User == null || !session.User.PermissionKeys.Any(c => c == _key))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
