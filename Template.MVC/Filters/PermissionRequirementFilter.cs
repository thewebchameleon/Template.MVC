using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Authentication;

namespace Template.MVC.Filters
{
    public class PermissionRequirementFilter : IAsyncAuthorizationFilter
    {
        private readonly string _key;

        public PermissionRequirementFilter(string key)
        {
            _key = key;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // todo: should store claims in session rather and check if the current session has the required claim

            var hasPermission = context.HttpContext.User.Claims.Any(c => c.Type == PermissionConstants.UserPermission && c.Value == _key);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
