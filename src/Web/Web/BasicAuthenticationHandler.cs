using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace CodeMooc.Web {

    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions> {

        protected readonly IConfiguration Configuration;

        public BasicAuthenticationHandler(
            IConfiguration configuration,
            IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        )
            : base(options, logger, encoder, clock) {

            Configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
            if(!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeader)) {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header not set or not readable"));
            }

            Logger.LogDebug("Authorization header: {0}", authorizationHeader);

            var authContent = Convert.FromBase64String(authorizationHeader[0].Substring(6));
            var authFields = System.Text.Encoding.ASCII.GetString(authContent).Split(':', StringSplitOptions.None);
            if(authFields.Length != 2) {
                Logger.LogDebug("Authorization header invalid");
                return Task.FromResult(AuthenticateResult.Fail("Authorization header invalid"));
            }

            var username = authFields[0];
            var password = authFields[1];

            Logger.LogDebug("Authorizing user {0} with password {1}", username, password);

            var confSection = Configuration.GetSection("StaticUserPasswords");
            var passwordMap = confSection.GetChildren().ToDictionary(element => element.Key.ToLowerInvariant(), element => element.Value);
            if(!passwordMap.ContainsKey(username)) {
                Logger.LogDebug("User {0} not registered", username);
                return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));
            }
            if(!BCrypt.Net.BCrypt.Verify(password, passwordMap[username])) {
                Logger.LogDebug("Password doesn't match hash {0}", passwordMap[username]);
                return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));
            }

            Logger.LogDebug("Authorized successfully");

            var t = new AuthenticationTicket(new ClaimsPrincipal(new StaticUserIdentity(username)), BasicAuthenticationSchemeOptions.DefaultScheme);
            return Task.FromResult(AuthenticateResult.Success(t));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties) {
            if(!Request.Headers.ContainsKey(HeaderNames.Authorization)) {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                Response.Headers.Add(HeaderNames.WWWAuthenticate,
                    new StringValues("Basic realm=\"CodeMOOC.net\"")
                );
            }
            else {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }

            return Task.CompletedTask;
        }

    }

}
