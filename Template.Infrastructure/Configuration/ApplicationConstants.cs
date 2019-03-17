using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System;
using Template.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;

namespace Template.Infrastructure.Configuration
{
    public static class ApplicationConstants
    {
        public const int SystemUserId = 1;
        public const string CultureInfo = "en-ZA";
        public const int SessionTimeoutSeconds = 60 * 10; // 10 minutes

        public static IdentityOptions IdentityOptions
        {
            get
            {
                var options = new IdentityOptions();

                // User settings
                options.User.RequireUniqueEmail = true;

                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(60 * 30); // 30 minutes
                options.Lockout.MaxFailedAccessAttempts = 10;

                return options;
            }
        }

        public static CookieAuthenticationOptions CookieAuthenticationOptions
        {
            get
            {
                var options = new CookieAuthenticationOptions();

                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromSeconds(ApplicationConstants.SessionTimeoutSeconds);
                options.SlidingExpiration = true;

                return options;
            }
        }

        public static AuthorizationOptions AuthorizationOptions
        {
            get
            {
                var options = new AuthorizationOptions();

                //options.AddPolicy(Policies.ViewAllUsersPolicy, policy => policy.RequireClaim(ClaimConstants.Permission, ApplicationPermissions.ViewUsers));

                return options;
            }
        }
    }
}
