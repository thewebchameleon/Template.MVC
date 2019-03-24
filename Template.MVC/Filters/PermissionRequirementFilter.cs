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
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == ClaimConstants.UserPermission && c.Value == _key);
            if (!hasClaim)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
