using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Template.Infrastructure.Authentication;

namespace Template.Infrastructure.Authentication
{
    public class ClaimsHandler : AuthorizationHandler<ClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   ClaimsRequirement requirement)
        {
            if (!context.User.HasClaim(c =>
                    c.Type == ClaimConstants.UserPermission
                    && requirement.Claims.Contains(c.Value)
            ))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
