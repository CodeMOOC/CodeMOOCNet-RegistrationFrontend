using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        public IActionResult Index() {
            return View("Create");
        }

        [HttpPost()]
        public IActionResult Process(RegistrationViewModel model, [FromForm(Name = "g-recaptcha-response")] string recaptchaResponse) {
            Logger.LogInformation(LoggingEvents.Registration, "Received registration request");

            if (!ModelState.IsValid) {
                Logger.LogInformation(LoggingEvents.Registration, "Model binding failed");

                return View("Create", model);
            }

            // Check ReCaptcha
            Logger.LogTrace(LoggingEvents.Registration, "Checking ReCaptcha token");
            var rest = new RestClient("https://www.google.com/recaptcha/api/siteverify");
            var restReq = new RestRequest(Method.POST);
            restReq.AddParameter("response", recaptchaResponse);
            restReq.AddParameter("secret", Environment.GetEnvironmentVariable("GOOGLE_RECAPTCHA_SECRET"));
            var recaptchaResult = rest.Execute<ReCaptchaResponse>(restReq);
            if(!recaptchaResult.IsSuccessful || !recaptchaResult.Data.Success) {
                Logger.LogWarning(LoggingEvents.Registration, "ReCaptcha verification failed");

                return View("Create", model);
            }
            Logger.LogInformation(LoggingEvents.Registration, "ReCaptcha verification succeeded for hostname {0}", recaptchaResult.Data.Hostname);

            //Check e-mail
            var existingRegistration = (from r in Database.Context.Registrations
                                        where r.Email == model.Email.ToLowerInvariant()
                                        select r).SingleOrDefault();
            if (existingRegistration != null) {
                Logger.LogInformation(LoggingEvents.Registration, "E-mail already registered");
                ModelState.AddModelError(nameof(RegistrationViewModel.Email), "Indirizzo e-mail già registrato");

                return View("Create", model);
            }

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
                ConfirmationSecret = "abc"
            };
            Database.Context.Registrations.Add(user);
            int changes = Database.Context.SaveChanges();

            if(changes != 1) {
                Logger.LogError(LoggingEvents.Registration, "Database changes not equal to 1");
                return StatusCode(500, "Database registration error");
            }

            return Ok(true);
        }

        [HttpPost("verifica")]
        public IActionResult Validate([FromRoute] int id, [FromQuery] string secret) {
            return Ok(false);
        }

    }

}
