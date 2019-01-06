using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CodeMooc.Web.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace CodeMooc.Web.Controllers {

    [Route("")]
    public class RegisterController : Controller {

        protected DatabaseManager Database { get; }
        protected ILogger<RegisterController> Logger { get; }

        public RegisterController(
            DatabaseManager database,
            ILogger<RegisterController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        private string GenerateSecret() {
            var rnd = new Random();
            byte[] buffer = new byte[10];
            rnd.NextBytes(buffer);
            return Convert.ToBase64String(buffer, Base64FormattingOptions.None).Substring(0, 10);
        }

        private async Task SendConfirmationEmail(Data.Registration user) {
            string url = Url.Action(nameof(Validate), "Register", new { id = user.Id, secret = user.ConfirmationSecret });
            string link = "http://codemooc.net" + url;

            Logger.LogTrace(LoggingEvents.Email, "Destination URL {0}, final link {1}", url, link);

            using (var client = new SmtpClient()) {
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

                var noReplyAddress = new MailAddress("no-reply@codemooc.net", "CodeMOOC.net");
                var msg = new MailMessage {
                    From = noReplyAddress,
                    Subject = "Verifica indirizzo e-mail",
                    IsBodyHtml = false,
                    Body = $"Ciao {user.Name}!\n\nGrazie per esserti registrato/a su CodeMOOC.net. Ti preghiamo di verificare il tuo indirizzo e-mai cliccando sul seguente link:\n{link}\n\nA presto!"
                };
                msg.To.Add(new MailAddress(user.Email, $"{user.Name} {user.Surname}"));
                msg.ReplyToList.Add(noReplyAddress);

                Logger.LogTrace(LoggingEvents.Email, "Sending e-mail");

                try {
                    await client.SendMailAsync(msg);
                }
                catch(Exception ex) {
                    Logger.LogError(LoggingEvents.Email, ex, "Failed to send e-mail");
                }
            }
        }

        [HttpGet]
        public IActionResult Index() {
            return View("Create");
        }

        [HttpPost()]
        public IActionResult Process(RegistrationViewModel model, [FromForm(Name = "g-recaptcha-response")] string recaptchaResponse) {
            Logger.LogInformation(LoggingEvents.Registration, "Received registration request");

            // Check e-mail
            var existingMailUser = (from r in Database.Context.Registrations
                                    where r.Email == model.Email.ToLowerInvariant()
                                    select r).SingleOrDefault();
            if (existingMailUser != null) {
                Logger.LogInformation(LoggingEvents.Registration, "E-mail already registered");
                ModelState.AddModelError(nameof(RegistrationViewModel.Email), "Indirizzo e-mail già registrato");
            }

            // Check fiscal code
            var existingCodeUser = (from r in Database.Context.Registrations
                                    where r.FiscalCode == model.FiscalCode.ToUpperInvariant()
                                    select r).SingleOrDefault();
            if (existingCodeUser != null) {
                Logger.LogInformation(LoggingEvents.Registration, "Fiscal code already registered");
                ModelState.AddModelError(nameof(RegistrationViewModel.FiscalCode), "Codice fiscale già registrato");
            }

            // Check ReCaptcha
            Logger.LogTrace(LoggingEvents.Registration, "Checking ReCaptcha token");
            var rest = new RestClient("https://www.google.com/recaptcha/api/siteverify");
            var restReq = new RestRequest(Method.POST);
            restReq.AddParameter("response", recaptchaResponse);
            restReq.AddParameter("secret", Environment.GetEnvironmentVariable("GOOGLE_RECAPTCHA_SECRET"));
            var recaptchaResult = rest.Execute<ReCaptchaResponse>(restReq);
            if (!recaptchaResult.IsSuccessful || !recaptchaResult.Data.Success) {
                Logger.LogWarning(LoggingEvents.Registration, "ReCaptcha verification failed");
                ModelState.AddModelError("ReCaptcha", "Attiva il controllo anti-spam ReCaptcha");

                return View("Create", model);
            }
            Logger.LogInformation(LoggingEvents.Registration, "ReCaptcha verification succeeded for hostname {0}", recaptchaResult.Data.Hostname);

            if (!ModelState.IsValid) {
                Logger.LogInformation(LoggingEvents.Registration, "Model binding failed");

                return View("Create", model);
            }

            // Proceed
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password);

            Logger.LogDebug(LoggingEvents.Registration, "Password hashed to {0}, length {1}", hashedPassword, hashedPassword.Length);

            var user = new Data.Registration {
                Name = model.Name.Trim(),
                Surname = model.Surname.Trim(),
                Birthday = model.Birthday,
                Birthplace = model.Birthplace,
                FiscalCode = model.FiscalCode.ToUpperInvariant(),
                AddressStreet = model.AddressStreet,
                AddressCity = model.AddressCity,
                AddressCap = model.AddressCap,
                AddressCountry = model.AddressCountry,
                Email = model.Email.Trim().ToLowerInvariant(),
                PasswordSchema = "bcrypt.net", // hard-coded to hash function
                PasswordHash = hashedPassword,
                Category = model.Category,
                HasAttendedMooc = model.HasAttendedMooc,
                HasCompletedMooc = model.HasCompletedMooc,
                RegistrationTimestamp = DateTime.UtcNow,
                ConfirmationSecret = GenerateSecret()
            };
            Database.Context.Registrations.Add(user);
            int changes = Database.Context.SaveChanges();

            if(changes != 1) {
                throw new InvalidOperationException("Expected changes equal to 1 when registering user");
            }

            SendConfirmationEmail(user).Forget();

            // Confirmation view
            ViewData["email"] = user.Email;
            return View("Confirm");
        }

        [HttpPost("verifica")]
        public IActionResult Validate([FromRoute] int id, [FromQuery] string secret) {
            return Ok(false);
        }

    }

}
