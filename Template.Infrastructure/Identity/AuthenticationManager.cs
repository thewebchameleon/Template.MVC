using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Identity
{
    public class AuthenticationManager : SignInManager<User>
    {
        public AuthenticationManager(
            UserManager<User> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger,
            IAuthenticationSchemeProvider schemes)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }

        public async override Task<ClaimsPrincipal> CreateUserPrincipalAsync(User user)
        {
            var principal = await base.CreateUserPrincipalAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            // todo: add custom claims here
            //identity.AddClaim(new System.Security.Claims.Claim("UserId", $"{user.Id}"));

            return principal;
        }
    }
}
