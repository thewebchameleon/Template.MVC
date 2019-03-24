using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace Template.Infrastructure.Authentication
{
    public class ClaimsRequirement : IAuthorizationRequirement
    {
        public List<string> Claims { get; }

        public ClaimsRequirement(string claim) : this(new List<string>() { claim }) { }

        public ClaimsRequirement(params string[] claims) : this(claims.ToList()) { }

        public ClaimsRequirement(List<string> claims)
        {
            Claims = claims;
        }
    }
}
