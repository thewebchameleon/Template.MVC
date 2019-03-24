using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Template.Infrastructure.Authentication
{
    public class PermissionsHandler : AuthorizationHandler<PermissionsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   PermissionsRequirement requirement)
        {
            if (!context.User.HasClaim(c =>
                    c.Type == PermissionConstants.UserPermission
                    && requirement.Permissions.Contains(c.Value)
            ))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
