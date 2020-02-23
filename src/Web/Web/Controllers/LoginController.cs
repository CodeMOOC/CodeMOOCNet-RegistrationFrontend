using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeMooc.Web.Data;
using CodeMooc.Web.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web.Controllers {

    [Route("login")]
    public class LoginController : Controller {

        protected DataContext Database { get; }
        protected ILogger<LoginController> Logger { get; }

        public LoginController(
            DataContext database,
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

            var mailRecord = (from e in Database.Emails
                              where e.Address == email.ToLowerInvariant()
                              select e).SingleOrDefault();
            if(mailRecord == null) {
                Logger.LogDebug("E-mail address {0} not registered", email);
                return FailLoginAttempt(email);
            }

            var userRecord = (from u in Database.Registrations
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

        [HttpPost("logout")]
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

        private async Task SendPasswordResetEmail(string email, Data.Registration user) {
            string url = Url.Action(nameof(FinalizeResetPassword), "Login", new { id = user.Id, secret = user.PasswordResetSecret });
            string link = Environment.GetEnvironmentVariable("BASE_URL") + url;

            Logger.LogTrace(LoggingEvents.Email, "Destination URL {0}, final link {1}", url, link);

            using(var client = new SmtpClient()) {
                var credentials = new NetworkCredential(
                    Environment.GetEnvironmentVariable("SMTP_USERNAME"),
                    Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                );

                client.EnableSsl = true;
                client.Host = Environment.GetEnvironmentVariable("SMTP_HOST");
                client.Port = Convert.ToInt32(Environment.GetEnvironmentVariable("SMTP_PORT"));
                client.Credentials = credentials;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 5000;

                Logger.LogTrace(LoggingEvents.Email, "SMTP host {0}:{1} username {2} SSL {3}", client.Host, client.Port, credentials.UserName, client.EnableSsl);

                var noReplyAddress = new MailAddress(Environment.GetEnvironmentVariable("MAIL_FROM"), "CodeMOOC.net");
                var msg = new MailMessage {
                    From = noReplyAddress,
                    Subject = "Richiesta nuova password",
                    IsBodyHtml = false,
                    Body = $"Ciao {user.Name}!\n\nHai richiesto che la tua password di accesso a CodeMOOC.net sia reimpostata. Per proseguire con la procedura, ti preghiamo di cliccare sul seguente collegamento:\n{link}\n\nA presto!\nCodeMOOC.net"
                };
                msg.To.Add(new MailAddress(email, $"{user.Name} {user.Surname}"));
                if(!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CONFIRMATION_MAIL_BCC"))) {
                    msg.Bcc.Add(Environment.GetEnvironmentVariable("CONFIRMATION_MAIL_BCC"));
                }
                msg.ReplyToList.Add(noReplyAddress);

                Logger.LogTrace(LoggingEvents.Email, "Sending e-mail");

                try {
                    await client.SendMailAsync(msg);
                }
                catch(Exception ex) {
                    Logger.LogError(LoggingEvents.Email, ex, "Failed to send e-mail");
                }

                Logger.LogDebug(LoggingEvents.Email, "E-mail sent");
            }
        }

        [HttpGet("reset")]
        public IActionResult ResetPassword() {
            var model = TempData.Get<ResetPasswordViewModel>() ?? new ResetPasswordViewModel();

            return View("ResetPassword", model);
        }

        [HttpPost("reset")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformResetPassword([FromForm] string email) {
            var user = (from e in Database.Emails
                        where e.Address == email.ToLowerInvariant()
                        select e.Registration).SingleOrDefault();
            if(user == null) {
                Logger.LogError("Password reset requested for non-existing e-mail {0}", email);
                TempData.Put(new ResetPasswordViewModel {
                    PasswordResetRequested = true
                });
                return RedirectToAction(nameof(ResetPassword));
            }

            Logger.LogInformation("Password reset requested for {0}", email);
            user.PasswordResetSecret = Startup.GenerateSecret();
            var taskDb = Database.SaveChangesAsync();

            var taskMail = SendPasswordResetEmail(email, user);

            await Task.WhenAll(taskDb, taskMail);

            TempData.Put(new ResetPasswordViewModel {
                PasswordResetRequested = true
            });
            return RedirectToAction(nameof(ResetPassword));
        }

        [HttpGet("reset-complete")]
        public IActionResult FinalizeResetPassword(int? id, string secret) {
            var model = TempData.Get<CompleteResetPasswordViewModel>() ?? new CompleteResetPasswordViewModel();
            if(id.HasValue) {
                model.UserId = id.Value;
            }
            if(secret != null) {
                model.Secret = secret;
            }

            return View("CompleteResetPassword", model);
        }

        [HttpPost("reset-complete")]
        [ValidateAntiForgeryToken]
        public IActionResult PerformFinalizeResetPassword(
            [FromForm] int id,
            [FromForm] string secret,
            [FromForm] string password
        ) {
            var user = (from r in Database.Registrations
                        where r.Id == id
                        where r.PasswordResetSecret == secret
                        select r).SingleOrDefault();
            if(user == null) {
                TempData.Put(new ResetPasswordViewModel {
                    FailureDuringReset = true
                });
                return RedirectToAction(nameof(ResetPassword));
            }

            user.PasswordSchema = "bcrypt.net"; // hard-coded to hash function
            user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
            user.PasswordResetSecret = null;
            Database.SaveChanges();

            TempData.Put(new CompleteResetPasswordViewModel {
                PasswordReset = true
            });
            return RedirectToAction(nameof(FinalizeResetPassword));
        }

    }

}
