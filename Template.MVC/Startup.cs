using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Template.Infrastructure.Cache;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.Configuration.Models;
using Template.Infrastructure.Identity;
using Template.Infrastructure.UnitOfWork;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models;
using Template.Models.DomainModels;
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

            // add our config options directly to dependancy injection
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<ConnectionStringSettings>>().Value);
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<CacheSettings>>().Value);

            // Configure services
            services.AddTransient<IUserStore<User>, UserManager>();
            services.AddTransient<IRoleStore<Role>, RoleManager>();

            services.AddTransient<ICacheProvider, MemoryCacheProvider>();
            services.AddTransient<IEntityCache, EntityCache>();
            services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();

            //services.AddTransient<IUserServiceManager, UserSerivceManager>();

            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IEmailService, EmailService>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // add identity
            services.AddIdentity<User, Role>()
                .AddSignInManager<AuthenticationManager>()
                .AddDefaultTokenProviders();

            // Configure Identity options and password complexity here
            services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;

                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
            });

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.ViewAllUsersPolicy, policy => policy.RequireClaim(ClaimConstants.Permission, ApplicationPermissions.ViewUsers));
            });

            services.AddMemoryCache();

            services.AddMvc()
#if DEBUG
                // todo: this is causing routing to throw an exception - https://github.com/aspnet/AspNetCore/issues/7647
                //.AddRazorRuntimeCompilation()
#endif
                .AddNewtonsoftJson();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(routes =>
            {
                routes.MapApplication();
                routes.MapControllerRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseCookiePolicy();

            app.UseAuthorization();
            app.UseAuthentication();
        }
    }
}
