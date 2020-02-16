// Browser incompatibility due to new SameSite cookie standard change
// is resolved in this file. 
// Special thanks to the authors of these articles:
// https://www.thinktecture.com/identity/samesite/prepare-your-identityserver/#does-this-affect-me-and-if-yes-how
// https://devblogs.microsoft.com/aspnet/upcoming-samesite-cookie-changes-in-asp-net-and-asp-net-core/


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SameSiteCookiesCompatibility
    {
        public static IServiceCollection ConfigureSameSiteCookiesCompatibility(this IServiceCollection services)
        {
            // Adding custom cookie policy for user agent sniffing to resolve
            // browser incompatibility issues due to the new standard of
            // SameSite cookie extension
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext => CheckSameSiteCompatibility(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext => CheckSameSiteCompatibility(cookieContext.Context, cookieContext.CookieOptions);
            });

            return services;
        }

        private static void CheckSameSiteCompatibility(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

                if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }

                if (userAgent.Contains("Safari") && userAgent.Contains("Macintosh; Intel Mac OS X 10_14") && userAgent.Contains("Version/"))
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }

                if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }
    }
}
