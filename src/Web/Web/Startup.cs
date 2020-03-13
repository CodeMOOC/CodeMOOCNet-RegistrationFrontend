using System;
using System.IO;
using CodeMooc.Web.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

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

        public const string CorsPolicyCodeMooc = "CodeMoocCORS";

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();

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

            services.AddCors(opts => {
                opts.AddPolicy(CorsPolicyCodeMooc, builder => {
                    builder.WithOrigins("https://*.codemooc.net")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithHeaders(HeaderNames.Authorization);
                });
            });

            // Add services to dependency registry
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<DataContext>(o => {
                var dbSection = Configuration.GetSection("Database");
                var host = dbSection["Host"];
                var port = Convert.ToInt32(dbSection["Port"]);
                var username = dbSection["Username"];
                var password = dbSection["Password"];
                var schema = dbSection["Schema"];
                var connectionString = string.Format(
                    "server={0};port={1};uid={2};pwd={3};database={4};Old Guids=false",
                    host, port, username, password, schema
                );

                o.UseMySQL(connectionString);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
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

            app.UseRouting();

            app.UseRequestLocalization(o => {
                o.AddSupportedCultures("it");
                o.AddSupportedUICultures("it");
                o.DefaultRequestCulture = new RequestCulture("it");
            });

            app.UseStaticFiles();

            // Add static file paths for uploaded files
            var pathsConf = Configuration.GetSection("Paths");

            string pathCurricula = pathsConf["Curricula"];
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(Path.Combine("/data", pathCurricula)),
                RequestPath = "/uploads/curricula"
            });

            string pathProfilePics = pathsConf["ProfilePics"];
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(Path.Combine("/data", pathProfilePics)),
                RequestPath = "/uploads/profiles"
            });

            app.UseCors(CorsPolicyCodeMooc);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(conf => {
                conf.MapControllers();
            });
        }

        private static readonly Random _random = new Random();

        public static string GenerateSecret() {
            byte[] buffer = new byte[10];
            _random.NextBytes(buffer);
            return Convert.ToBase64String(buffer, Base64FormattingOptions.None).Substring(0, 10);
        }

    }

}
