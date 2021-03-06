﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CodeMooc.Web.Data;
using CodeMooc.Web.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace CodeMooc.Web.Controllers {

    [Route("iscrizione")]
    public class RegisterController : Controller {

        protected DataContext Database { get; }
        protected ILogger<RegisterController> Logger { get; }

        public RegisterController(
            DataContext database,
            ILogger<RegisterController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        private async Task SendConfirmationEmail(Data.Registration user, Data.Email email) {
            string url = Url.Action(nameof(Validate), "Register", new { id = user.Id, secret = user.ConfirmationSecret });
            string link = Environment.GetEnvironmentVariable("BASE_URL") + url;

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

                var noReplyAddress = new MailAddress(Environment.GetEnvironmentVariable("MAIL_FROM"), "CodeMOOC.net");
                var msg = new MailMessage {
                    From = noReplyAddress,
                    Subject = "Verifica indirizzo e-mail",
                    IsBodyHtml = false,
                    Body = $"Ciao {user.Name} {user.Surname}!\n\nGrazie per esserti registrato/a su CodeMOOC.net. Ti preghiamo di verificare il tuo indirizzo e-mail cliccando sul seguente link:\n{link}\n\nA presto!\nCodeMOOC.net"
                };
                msg.To.Add(new MailAddress(email.Address, $"{user.Name} {user.Surname}"));
                if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CONFIRMATION_MAIL_BCC"))) {
                    msg.Bcc.Add(Environment.GetEnvironmentVariable("CONFIRMATION_MAIL_BCC"));
                }
                msg.ReplyToList.Add(noReplyAddress);

                Logger.LogTrace(LoggingEvents.Email, "Sending e-mail");

                try {
                    await client.SendMailAsync(msg);
                }
                catch (Exception ex) {
                    Logger.LogError(LoggingEvents.Email, ex, "Failed to send e-mail");
                }

                Logger.LogDebug(LoggingEvents.Email, "E-mail sent");
            }
        }

        [HttpGet]
        public IActionResult Index() {
            return View("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Process(RegistrationViewModel model, [FromForm(Name = "g-recaptcha-response")] string recaptchaResponse) {
            Logger.LogInformation(LoggingEvents.Registration, "Received registration request");

            // Check e-mail
            if (!string.IsNullOrWhiteSpace(model.Email)) {
                var existingMailUser = (from e in Database.Emails
                                        where e.Address == model.Email.ToLowerInvariant()
                                        select e).SingleOrDefault();
                if (existingMailUser != null) {
                    Logger.LogInformation(LoggingEvents.Registration, "E-mail already registered");
                    ModelState.AddModelError(nameof(RegistrationViewModel.Email), "Indirizzo e-mail già registrato");
                }
            }

            // Check fiscal code
            if (!string.IsNullOrWhiteSpace(model.FiscalCode)) {
                var existingCodeUser = (from r in Database.Registrations
                                        where r.FiscalCode == model.FiscalCode.ToUpperInvariant()
                                        where r.ConfirmationTimestamp != null
                                        select r).SingleOrDefault();
                if (existingCodeUser != null) {
                    Logger.LogInformation(LoggingEvents.Registration, "Fiscal code already registered");
                    ModelState.AddModelError(nameof(RegistrationViewModel.FiscalCode), "Codice fiscale già registrato");
                }
            }

            // Check birthday
            if (!DateTime.TryParseExact(model.Birthday, "dd-MM-yyyy", new CultureInfo("it-IT"), DateTimeStyles.AssumeLocal, out var birthday)) {
                Logger.LogInformation(LoggingEvents.Registration, "Birthday {0} not valid", model.Birthday);
                ModelState.AddModelError(nameof(RegistrationViewModel.Birthday), "Inserire data di nascita nel formato GG-MM-AAAA");
            }

            // Check ReCaptcha
            Logger.LogTrace(LoggingEvents.Registration, "Checking ReCaptcha token");
            var rest = new RestClient("https://www.google.com/recaptcha/api/siteverify");
            var restReq = new RestRequest(Method.POST);
            restReq.AddParameter("response", recaptchaResponse);
            restReq.AddParameter("secret", Environment.GetEnvironmentVariable("GOOGLE_RECAPTCHA_SECRET"));
            var recaptchaResult = rest.Execute<ReCaptchaResponse>(restReq);
            if (!recaptchaResult.IsSuccessful || !recaptchaResult.Data.Success) {
                Logger.LogInformation(LoggingEvents.Registration, "ReCaptcha verification failed");
                ModelState.AddModelError("ReCaptcha", "Attiva il controllo anti-spam ReCaptcha");

                return View("Create", model);
            }
            Logger.LogDebug(LoggingEvents.Registration, "ReCaptcha verification succeeded for hostname {0}", recaptchaResult.Data.Hostname);

            // Final validity check
            if (!ModelState.IsValid) {
                Logger.LogDebug(LoggingEvents.Registration, "Model binding failed");

                return View("Create", model);
            }

            Logger.LogDebug("Input validated, proceeding with registration");
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password);

            var user = new Data.Registration {
                Name = model.Name.Trim(),
                Surname = model.Surname.Trim(),
                Birthday = birthday.Date,
                Birthplace = model.Birthplace,
                FiscalCode = model.FiscalCode.ToUpperInvariant(),
                AddressStreet = model.AddressStreet,
                AddressCity = model.AddressCity,
                AddressCap = model.AddressCap,
                AddressCountry = model.AddressCountry,
                PasswordSchema = "bcrypt.net", // hard-coded to hash function
                PasswordHash = hashedPassword,
                Category = model.Category,
                HasAttendedMooc = model.HasAttendedMooc,
                HasCompletedMooc = model.HasCompletedMooc,
                RegistrationTimestamp = DateTime.UtcNow,
                ConfirmationSecret = Startup.GenerateSecret()
            };
            Database.Registrations.Add(user);

            var email = new Data.Email {
                Address = model.Email.Trim().ToLowerInvariant(),
                IsPrimary = true,
                AssociationTimestamp = user.RegistrationTimestamp,
                Registration = user
            };
            Database.Emails.Add(email);

            int changes = Database.SaveChanges();
            if(changes != 2) {
                throw new InvalidOperationException("Expected changes equal to 2 when registering user");
            }

            await SendConfirmationEmail(user, email);

            // Confirmation view
            ViewData["email"] = email.Address;
            return View("Confirm");
        }

        [HttpGet("verifica/{id}")]
        public IActionResult Validate([FromRoute] int id, [FromQuery] string secret) {
            var user = (from r in Database.Registrations
                        where r.Id == id
                        select r).SingleOrDefault();

            if (user == null) {
                Logger.LogInformation(LoggingEvents.Verification, "User #{0} not found", id);
                return StatusCode(404);
            }

            if (user.ConfirmationTimestamp != null) {
                Logger.LogDebug(LoggingEvents.Verification, "User {0} verified", id);
                return View("Validated");
            }

            if (!user.ConfirmationSecret.Equals(secret, StringComparison.InvariantCulture)) {
                Logger.LogInformation(LoggingEvents.Verification, "Secrets do not match {0} != {1}", secret, user.ConfirmationSecret);
                return StatusCode(404);
            }

            Logger.LogDebug(LoggingEvents.Verification, "Returning verification page for user {0}", id);

            ViewData["id"] = id;
            ViewData["secret"] = secret;
            return View("Validate");
        }

        [HttpPost("verifica/{id}")]
        public IActionResult DoValidate([FromRoute] int id, [FromForm] string secret) {
            var user = (from r in Database.Registrations
                        where r.Id == id
                        where r.ConfirmationTimestamp == null
                        select r).SingleOrDefault();

            if (user == null) {
                Logger.LogInformation(LoggingEvents.Verification, "User #{0} not found", id);
                return StatusCode(404);
            }

            if (!user.ConfirmationSecret.Equals(secret, StringComparison.InvariantCulture)) {
                Logger.LogInformation(LoggingEvents.Verification, "Secrets do not match {0} != {1}", secret, user.ConfirmationSecret);
                return StatusCode(404);
            }

            user.ConfirmationTimestamp = DateTime.UtcNow;
            Database.SaveChanges();

            Logger.LogInformation(LoggingEvents.Verification, "User {0} verified", id);

            return RedirectToAction("Validate", new { id, secret });
        }

        [HttpGet("mostra/{id}")]
        public IActionResult Show([FromRoute] int id) {
            var user = (from r in Database.Registrations
                        where r.Id == id
                        select r).SingleOrDefault();

            if (user == null) {
                return StatusCode(404);
            }

            user.ConfirmationSecret = null; // Do not show confirmation secret! 🕵️‍
            return Ok(user);
        }

    }

}
