using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Template.Infrastructure.Cache;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.Configuration;
using Template.Infrastructure.Configuration.Models;
using Template.Infrastructure.Email;
using Template.Infrastructure.Email.Contracts;
using Template.Infrastructure.Session;
using Template.Infrastructure.Session.Contracts;
using Template.Infrastructure.UnitOfWork;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.MVC.Filters;
using Template.MVC.Middleware;
using Template.Services;
using Template.Services.Contracts;

namespace Template.MVC
{
    public class Startup
    {
        private IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Add our config options
            services.Configure<ConnectionStringSettings>(_configuration.GetSection("ConnectionStrings"));
            services.Configure<CacheSettings>(_configuration.GetSection("Cache"));
            services.Configure<EmailSettings>(_configuration.GetSection("Email"));

            // add our config options directly to dependancy injection
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<ConnectionStringSettings>>().Value);
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<CacheSettings>>().Value);
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<EmailSettings>>().Value);

            // Configure services
            services.AddTransient<ISessionProvider, SessionProvider>();
            services.AddTransient<ICacheProvider, MemoryCacheProvider>();
            services.AddTransient<IEmailProvider, EmailProvider>();

            services.AddTransient<IApplicationCache, ApplicationCache>();
            services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddTransient<IHomeService, HomeService>();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDistributedMemoryCache();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;

                // Although this setting breaks OAuth2 and other cross-origin authentication schemes, 
                // it elevates the level of cookie security for other types of apps that don't rely 
                // on cross-origin request processing.
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            // authentication and authorization
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromSeconds(ApplicationConstants.SessionTimeoutSeconds);
                options.SlidingExpiration = true;

                options.Cookie = ApplicationConstants.SecureNamelessCookie;
                options.Cookie.Name = "Authentication";
            });
            services.AddAuthorization(options => { });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(ApplicationConstants.SessionTimeoutSeconds);

                options.Cookie = ApplicationConstants.SecureNamelessCookie;
                options.Cookie.Name = "Session";
            });

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
                options.EnableForHttps = true; // https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression#compression-with-secure-protocol
            });
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(SessionRequirementFilter));
                options.Filters.Add(typeof(SessionLoggingFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRequestLocalization(ApplicationConstants.CultureInfo);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseResponseCompression();
            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] =
                        "public,max-age=" + ApplicationConstants.ResponseCachingSeconds;
                }
            });

            app.UseCookiePolicy();
            app.UseSession();
            app.UseSessionMiddleware();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
