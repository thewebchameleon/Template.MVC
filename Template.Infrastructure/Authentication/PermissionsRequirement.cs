using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace Template.Infrastructure.Authentication
{
    public class PermissionsRequirement : IAuthorizationRequirement
    {
        public List<string> Permissions { get; }

        public PermissionsRequirement(string permission) : this(new List<string>() { permission }) { }

        public PermissionsRequirement(params string[] permissions) : this(permissions.ToList()) { }

        public PermissionsRequirement(List<string> permissions)
        {
            Permissions = permissions;
        }
    }
}
