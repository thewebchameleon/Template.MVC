using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Template.Infrastructure.Configuration
{
    public static class ApplicationConstants
    {
        public const int SystemUserId = 1;
        public const string CultureInfo = "en-ZA";
        public const int SessionTimeoutSeconds = 60 * 10; // 10 minutes

        public static CookieBuilder SecureNamelessCookie
        {
            get
            {
                return new CookieBuilder()
                {
                    SameSite = SameSiteMode.Strict,
                    SecurePolicy = CookieSecurePolicy.Always,
                    IsEssential = true,
                    HttpOnly = false
                };
            }
        }

        public static List<string> ObfuscatedActionArgumentFields
        {
            get
            {
                return new List<string>()
                {
                    "Password", "password"
                };
            }
        }
    }
}
