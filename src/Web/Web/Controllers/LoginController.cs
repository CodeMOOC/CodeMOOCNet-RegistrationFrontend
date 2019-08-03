using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeMooc.Web.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web.Controllers {

    [Route("login")]
    public class LoginController : Controller {

        protected DatabaseManager Database { get; }
        protected ILogger<LoginController> Logger { get; }

        public LoginController(
            DatabaseManager database,
            ILogger<LoginController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string proceed = null) {
            var model = TempData.Get<LoginViewModel>() ?? new LoginViewModel();
            model.ProceedUrl = proceed;
            model.IsLoggedIn = HttpContext.User.Identity.IsAuthenticated;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformLogin(
            [FromForm] string email,
            [FromForm] string password,
            [FromForm] bool remember,
            [FromForm] string proceed
        ) {
            Logger.LogDebug("Login attempt by {0}", email);

            var mailRecord = (from e in Database.Context.Emails
                              where e.Address == email.ToLowerInvariant()
                              select e).SingleOrDefault();
            if(mailRecord == null) {
                Logger.LogDebug("E-mail address {0} not registered", email);
                return FailLoginAttempt(email);
            }

            var userRecord = (from u in Database.Context.Registrations
                              where u.Id == mailRecord.RegistrationId
                              select u).Single();
            Logger.LogDebug("Matching {0} with {1}", password, userRecord.PasswordHash);
            if(!BCrypt.Net.BCrypt.EnhancedVerify(password, userRecord.PasswordHash)) {
                Logger.LogDebug("Login for user {0} {1} failed", userRecord.Id, mailRecord.Address);
                return FailLoginAttempt(email);
            }

            Logger.LogInformation("User {0} {1} logged in", userRecord.Id, mailRecord.Address);

            // TODO: check for association status

            var claims = new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userRecord.Id.ToString()),
                new Claim(ClaimTypes.GivenName, userRecord.Name),
                new Claim(ClaimTypes.Surname, userRecord.Surname),
                new Claim(ClaimTypes.DateOfBirth, userRecord.Birthday.ToString("s")),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, "Member")
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
                ),
                new AuthenticationProperties {
                    AllowRefresh = true,
                    IsPersistent = remember
                }
            );

            if(string.IsNullOrWhiteSpace(proceed)) {
                return RedirectToAction("Index", "Dashboard");
            }
            else {
                return LocalRedirect(proceed);
            }
        }

        protected IActionResult FailLoginAttempt(string email = "") {
            TempData.Put(new LoginViewModel {
                Email = email,
                Status = LoginViewModel.LoginStatus.LoginFailure
            });
            return RedirectToAction("Index");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout() {
            // Get email of logged in user
            var email = HttpContext.User?.FindFirstValue(ClaimTypes.Email);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData.Put(new LoginViewModel {
                Email = email,
                Status = LoginViewModel.LoginStatus.LoggedOut
            });
            return RedirectToAction("Index");
        }

        [HttpGet("unauthorized")]
        public IActionResult ShowUnauthorized() {
            return View("Unauthorized");
        }

    }

}
