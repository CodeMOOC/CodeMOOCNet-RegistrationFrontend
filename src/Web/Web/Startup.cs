using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMooc.Web {

    public class Startup {

        public Startup(IConfiguration configuration) {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public const string AdministratorsOnlyPolicyName = "AdministratorsOnly";
        public const string MembersOnlyPolicyName = "MembersOnly";
        public const string LegacyBasicAdministratorsPolicyName = "BasicAdministratorsOnly";

        public const string AdministratorRole = "Administrator";
        public const string MemberRole = "Member";

        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc(opts => {
                // None
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication(opts => {
                opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddScheme<BasicAuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationSchemeOptions.SchemeName, opts => {
                // No-opts
            })
            .AddCookie(opts => {
                opts.LoginPath = "/login";
                opts.LogoutPath = "/login/logout";
                opts.ReturnUrlParameter = "proceed";
                opts.AccessDeniedPath = "/login/unauthorized";
                opts.ExpireTimeSpan = TimeSpan.FromDays(30);
                opts.SlidingExpiration = true;
                opts.Cookie = new CookieBuilder {
                    Expiration = TimeSpan.FromDays(30),
                    Domain = "codemooc.net",
                    IsEssential = true,
                    Name = "CodeMOOCLogin",
                    SecurePolicy = CookieSecurePolicy.None,
                    SameSite = SameSiteMode.None,
                    HttpOnly = true
                };
            });

            services.AddAuthorization(opt => {
                opt.AddPolicy(
                    AdministratorsOnlyPolicyName,
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireRole(AdministratorRole)
                        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                        .Build()
                );
                opt.AddPolicy(
                    MembersOnlyPolicyName,
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireRole(MemberRole)
                        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                        .Build()
                );
                opt.AddPolicy(
                    LegacyBasicAdministratorsPolicyName,
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireRole(AdministratorRole)
                        .AddAuthenticationSchemes(BasicAuthenticationSchemeOptions.SchemeName)
                        .Build()
                );
            });

            // Add services to dependency registry
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<DatabaseManager>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // Fix incoming base path for hosting behind proxy
            string basePath = Environment.GetEnvironmentVariable("ASPNETCORE_BASEPATH");
            if (!string.IsNullOrEmpty(basePath)) {
                app.UsePathBase(new PathString(basePath));
                app.Use(async (context, next) => {
                    context.Request.PathBase = basePath;
                    await next.Invoke();
                });
            }

            app.UseRequestLocalization(o => {
                o.AddSupportedCultures("it");
                o.AddSupportedUICultures("it");
                o.DefaultRequestCulture = new RequestCulture("it");
            });

            app.UseStaticFiles("/static");

            app.UseAuthentication();

            app.UseMvc();
        }

    }

}
