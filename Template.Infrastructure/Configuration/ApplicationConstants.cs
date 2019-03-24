using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

namespace Template.Infrastructure.Configuration
{
    public static class ApplicationConstants
    {
        public const int SystemUserId = 1;
        public const string CultureInfo = "en-ZA";
        public const int SessionTimeoutSeconds = 60 * 10; // 10 minutes

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

        public static AuthenticationProperties AuthenticationProperties
        {
            get
            {
                var properties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddSeconds(ApplicationConstants.SessionTimeoutSeconds),
                    IssuedUtc = DateTime.UtcNow
                };

                return properties;
            }
        }
    }
}
