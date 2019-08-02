using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index(string proceed = null) {
            ViewData["proceed"] = proceed;
            ViewData["failure"] = TempData["failure"];

            // Clear existing cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformLogin(
            [FromForm] string email,
            [FromForm] string password,
            [FromForm] bool remember,
            [FromForm] string proceed
        ) {
            Logger.LogInformation("Logging {0} with pwd {1}, remember {2}, proceeding to {3}", email, password, remember, proceed);

            if(!email.Equals("asd", StringComparison.InvariantCultureIgnoreCase)) {
                TempData["failure"] = true;
                return RedirectToAction("Index");
            }

            var claims = new Claim[] {
                new Claim(ClaimTypes.Name, "Pinco Pallino"),
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

        [HttpGet("logout")]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }

        [HttpGet("unauthorized")]
        public IActionResult ShowUnauthorized() {
            return View("Unauthorized");
        }

    }

}
